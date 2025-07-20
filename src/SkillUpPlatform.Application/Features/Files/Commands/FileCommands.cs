using MediatR;
using SkillUpPlatform.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace SkillUpPlatform.Application.Features.Files.Commands;

public class UploadFileCommand : IRequest<Result<FileUploadDto>>
{
    public int UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = new byte[0];
    public string FileType { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public IFormFile? File { get; set; }
}

public class DeleteFileCommand : IRequest<Result<bool>>
{
    public int FileId { get; set; }
    public int UserId { get; set; }
}

public class ShareFileCommand : IRequest<Result<bool>>
{
    public int FileId { get; set; }
    public int SharedWithUserId { get; set; }
    public int SharedByUserId { get; set; }
    public string AccessLevel { get; set; } = "Read";
}

public class UpdateFileCommand : IRequest<Result<FileInfoDto>>
{
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}

public class FileUploadDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public List<FileShareDto> SharedWith { get; set; } = new();
}

public class FileShareDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public List<string> SharedWithUsers { get; set; } = new();
    public string Permission { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string ShareUrl { get; set; } = string.Empty;
}

public class UploadMultipleFilesCommand : IRequest<Result<List<FileUploadDto>>>
{
    public int UserId { get; set; }
    public List<byte[]> FilesContent { get; set; } = new();
    public List<string> FileNames { get; set; } = new();
    public List<string> FileTypes { get; set; } = new();
    public string? Category { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Description { get; set; }
    public List<IFormFile>? Files { get; set; }
}
