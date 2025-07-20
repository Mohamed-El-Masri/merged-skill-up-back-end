using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Files.Commands;
using SkillUpPlatform.Application.Features.Files.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileShare = SkillUpPlatform.Domain.Entities.FileShare;

namespace SkillUpPlatform.Application.Features.Files.Handlers
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<FileUploadDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public UploadFileCommandHandler(IUnitOfWork unitOfWork,
                                        IFileService fileService,
                                        IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }



        public async Task<Result<FileUploadDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate user exists
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return Result<FileUploadDto>.Failure("User not found.");
                }

                // Validate file content
                byte[] fileContent;
                if (request.File != null)
                {
                    using var memoryStream = new MemoryStream();
                    await request.File.CopyToAsync(memoryStream, cancellationToken);
                    fileContent = memoryStream.ToArray();
                }
                else
                {
                    fileContent = request.FileContent;
                }

                if (fileContent.Length == 0)
                {
                    return Result<FileUploadDto>.Failure("No file content provided.");
                }

                // Validate file type
                if (!_fileService.IsValidFileType(request.FileType))
                {
                    return Result<FileUploadDto>.Failure("Invalid file type.");
                }

                // Validate file size
                if (!_fileService.IsValidFileSize(fileContent.Length))
                {
                    return Result<FileUploadDto>.Failure("File size exceeds maximum allowed limit.");
                }

                // Save file to storage
                var uniqueFileName = await _fileService.SaveFileAsync(fileContent, request.OriginalFileName, request.FileType);
                var filePath = uniqueFileName;
                var fileUrl = await _fileService.GetFileUrlAsync(filePath);

                // Map UploadFileCommand to FileUpload entity
                var fileUpload = _mapper.Map<FileUpload>(request);
                fileUpload.FilePath = filePath;
                fileUpload.FileSize = fileContent.Length;
                fileUpload.UploadedAt = DateTime.UtcNow;

                // Save to database
                await _unitOfWork.FileUploadRepository.AddAsync(fileUpload);
                await _unitOfWork.SaveChangesAsync();

                // Map FileUpload to FileUploadDto
                var fileUploadDto = _mapper.Map<FileUploadDto>(fileUpload);
                fileUploadDto.FilePath = fileUrl; // Override FilePath with the file URL

                return Result<FileUploadDto>.Success(fileUploadDto);
            }
            catch (Exception ex)
            {
                return Result<FileUploadDto>.Failure($"Failed to upload file: {ex.Message}");
            }
        }
    }

    public class UploadMultipleFilesCommandHandler : IRequestHandler<UploadMultipleFilesCommand, Result<List<FileUploadDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public UploadMultipleFilesCommandHandler(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Result<List<FileUploadDto>>> Handle(UploadMultipleFilesCommand request, CancellationToken cancellationToken)
        {
            if (request.Files == null || !request.Files.Any())
                return Result<List<FileUploadDto>>.Failure("No files provided");

            // التحقق من وجود المستخدم
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<List<FileUploadDto>>.Failure($"User with ID {request.UserId} not found");

            var uploadedFiles = new List<FileUpload>();
            var errors = new List<string>();

            foreach (var file in request.Files)
            {
                if (!_fileService.IsValidFileType(file.ContentType))
                {
                    errors.Add($"Invalid file type: {file.FileName}");
                    continue;
                }

                if (!_fileService.IsValidFileSize(file.Length))
                {
                    errors.Add($"File size exceeds limit: {file.FileName}");
                    continue;
                }

                try
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    var fileContent = memoryStream.ToArray();

                    var uniqueFileName = await _fileService.SaveFileAsync(fileContent, file.FileName, file.ContentType);

                    var fileUpload = new FileUpload
                    {
                        UserId = user.Id,
                        FileName = uniqueFileName,
                        OriginalFileName = file.FileName,
                        FilePath = uniqueFileName,
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        UploadedBy = request.UserId,
                        UploadedAt = DateTime.UtcNow,
                        IsPublic = request.IsPublic,
                        Description = request.Description
                    };

                    uploadedFiles.Add(fileUpload);
                    await _unitOfWork.FileUploadRepository.AddAsync(fileUpload);
                }
                catch (Exception)
                {
                    errors.Add($"Failed to upload file: {file.FileName}");
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<List<FileUploadDto>>(uploadedFiles);

                for (int i = 0; i < result.Count; i++)
                {
                    try
                    {
                        result[i].FilePath = await _fileService.GetFileUrlAsync(uploadedFiles[i].FilePath);
                    }
                    catch (Exception)
                    {
                        errors.Add($"Failed to retrieve file URL for: {uploadedFiles[i].FileName}");
                    }
                }

                if (errors.Any())
                    return Result<List<FileUploadDto>>.Failure(result + string.Join("; ", errors));

                return Result<List<FileUploadDto>>.Success(result);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("FOREIGN KEY constraint") == true)
                {
                    return Result<List<FileUploadDto>>.Failure($"Foreign key constraint violation: User ID {request.UserId} does not exist in the database");
                }
                return Result<List<FileUploadDto>>.Failure($"Database error occurred while saving files: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception)
            {
                return Result<List<FileUploadDto>>.Failure("An unexpected error occurred while processing the files");
            }
        }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public DeleteFileCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var file = await _unitOfWork.FileUploadRepository.GetByIdAsync(request.FileId);
                if (file == null)
                    return Result<bool>.Failure("File not found");

                if (file.UploadedBy != request.UserId)
                    return Result<bool>.Failure("Unauthorized to delete this file");

                var deleted = await _fileService.DeleteFileAsync(file.FilePath);
                if (!deleted)
                    return Result<bool>.Failure("Failed to delete file from storage");

                _unitOfWork.FileUploadRepository.Remove(file);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("An error occurred while deleting the file");
            }
        }
    }

    public class GetFileDownloadQueryHandler : IRequestHandler<GetFileDownloadQuery, Result<FileDownloadDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public GetFileDownloadQueryHandler(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Result<FileDownloadDto>> Handle(GetFileDownloadQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var file = await _unitOfWork.FileUploadRepository.GetByIdAsync(request.FileId);
                if (file == null)
                    return Result<FileDownloadDto>.Failure("File not found");

                if (!file.IsPublic && file.UploadedBy != request.UserId)
                {
                    var shared = await _unitOfWork.FileUploadRepository.FindAsync(fs =>
                        fs.FileShares.Any(s => s.FileUploadId == request.FileId && s.SharedWithUserId == request.UserId));
                    if (!shared.Any())
                        return Result<FileDownloadDto>.Failure("Access denied");
                }

                var fileContent = await _fileService.GetFileAsync(file.FilePath);
                var result = _mapper.Map<FileDownloadDto>(file);
                result.FileContent = fileContent;

                return Result<FileDownloadDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<FileDownloadDto>.Failure($"File Not downloaded, {ex.Message}");
            }
        }
    }

    public class GetUserFilesQueryHandler : IRequestHandler<GetUserFilesQuery, Result<PagedResult<FileInfoDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper;

        public GetUserFilesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<FileInfoDto>>> Handle(GetUserFilesQuery request, CancellationToken cancellationToken)
        {
            var files = await _unitOfWork.FileUploadRepository.GetByUserIdAsync(request.UserId);

            if (!string.IsNullOrEmpty(request.Category))
                files = files.Where(f => f.FileType == request.Category).ToList();

            var totalCount = files.Count;
            var pagedFiles = files
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = new PagedResult<FileInfoDto>
            {
                Data = _mapper.Map<List<FileInfoDto>>(pagedFiles),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Result<PagedResult<FileInfoDto>>.Success(result);
        }
    }

    public class GetFileDetailsQueryHandler : IRequestHandler<GetFileDetailsQuery, Result<FileDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork; private readonly IFileService _fileService; private readonly IMapper _mapper;

        public GetFileDetailsQueryHandler(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Result<FileDetailsDto>> Handle(GetFileDetailsQuery request, CancellationToken cancellationToken)
        {
            var file = await _unitOfWork.FileUploadRepository.GetByIdAsync(request.FileId);
            if (file == null)
                return Result<FileDetailsDto>.Failure("File not found");

            if (!file.IsPublic && file.UploadedBy != request.UserId)
            {
                var shared = await _unitOfWork.FileUploadRepository.FindAsync(fs =>
                    fs.FileShares.Any(s => s.FileUploadId == request.FileId && s.SharedWithUserId == request.UserId));
                if (!shared.Any())
                    return Result<FileDetailsDto>.Failure("Access denied");
            }

            var result = _mapper.Map<FileDetailsDto>(file);
            result.Url = await _fileService.GetFileUrlAsync(file.FilePath);

            return Result<FileDetailsDto>.Success(result);
        }
    }

    public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, Result<FileInfoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<FileInfoDto>> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var file = await _unitOfWork.FileUploadRepository.GetByIdAsync(request.FileId);
            if (file == null)
                return Result<FileInfoDto>.Failure("File not found");

            if (file.UploadedBy != request.UserId)
                return Result<FileInfoDto>.Failure("Unauthorized to update this file");

            file.FileName = request.FileName;
            file.Description = request.Description;
            file.IsPublic = request.IsPublic;

            _unitOfWork.FileUploadRepository.Update(file);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<FileInfoDto>(file);
            result.Category = request.Category;

            return Result<FileInfoDto>.Success(result);
        }
    }

    public class ShareFileCommandHandler : IRequestHandler<ShareFileCommand, Result<FileShareDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ShareFileCommandHandler(IUnitOfWork unitOfWork,
                                       IFileService fileService,
                                       IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Result<FileShareDto>> Handle(ShareFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var file = await _unitOfWork.FileUploadRepository.GetByIdAsync(request.FileId);
                if (file == null)
                    return Result<FileShareDto>.Failure("File not found");

                if (file.UploadedBy != request.SharedByUserId)
                    return Result<FileShareDto>.Failure("Unauthorized to share this file");

                var fileShare = new FileShare
                {
                    FileUploadId = request.FileId,
                    SharedWithUserId = request.SharedWithUserId,
                    SharedBy = request.SharedByUserId,
                    SharedAt = DateTime.UtcNow,
                    AccessLevel = request.AccessLevel
                };

                await _unitOfWork.FileShareRepository.AddAsync(fileShare);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<FileShareDto>(fileShare);
                result.FileName = file.FileName;
                result.ShareUrl = await _fileService.GetFileUrlAsync(file.FilePath);

                return Result<FileShareDto>.Success(result);
            }
            catch (Exception)
            {
                return Result<FileShareDto>.Failure("An error occurred while sharing the file");
            }
        }
    }

    public class GetFileCategoriesQueryHandler : IRequestHandler<GetFileCategoriesQuery, Result<List<FileCategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; private readonly IMapper _mapper;

        public GetFileCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<FileCategoryDto>>> Handle(GetFileCategoriesQuery request, CancellationToken cancellationToken)
        {
            var files = await _unitOfWork.FileUploadRepository.GetByUserIdAsync(request.UserId);

            var categories = files
                .Where(f => !string.IsNullOrEmpty(f.FileType))
                .GroupBy(f => f.FileType)
                .Select(g => new FileCategoryDto
                {
                    Name = g.Key,
                    FileCount = g.Count()
                })
                .ToList();

            return Result<List<FileCategoryDto>>.Success(categories);
        }
    }
}