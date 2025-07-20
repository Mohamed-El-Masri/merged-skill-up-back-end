using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Exceptions;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Contentt.Commands;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Application.Features.Contentt.Queries;
using SkillUpPlatform.Application.Features.Progress.Commands;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Features.Contentt.Handlers;

public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, Result<ContentDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentDetailDto>> Handle(CreateContentCommand request, CancellationToken cancellationToken)
    {
        var contentt = _mapper.Map<Content>(request);

        await _unitOfWork.Contents.AddAsync(contentt);
        await _unitOfWork.SaveChangesAsync();

        return Result<ContentDetailDto>.Success(_mapper.Map<ContentDetailDto>(contentt));
    }
}

public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(request.Id);
        if (content == null)
            return Result.Failure("Content not found");

        content.Title = request.Title;
        content.Description = request.Description;
        content.VideoUrl = request.VideoUrl;

        _unitOfWork.Contents.Update(content);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}


public class DeleteContentCommandHandler : IRequestHandler<DeleteContentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(request.Id);
        if (content == null)
            return Result.Failure("Content not found");

        _unitOfWork.Contents.Remove(content);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}


public class GetContentQueryHandler : IRequestHandler<GetContentQuery, Result<List<ContentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<ContentDto>>> Handle(GetContentQuery request, CancellationToken cancellationToken)
    {
        var contents = await _unitOfWork.Contents.GetAllAsync();
        var result = _mapper.Map<List<ContentDto>>(contents);
        return Result<List<ContentDto>>.Success(result);
    }
}

public class GetContentByIdQueryHandler : IRequestHandler<GetContentByIdQuery, Result<ContentDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentDetailDto>> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(request.ContentId);
        if (content == null)
            return Result<ContentDetailDto>.Failure("Content not found");

        var dto = _mapper.Map<ContentDetailDto>(content);
        return Result<ContentDetailDto>.Success(dto);
    }
}

public class GetContentByLearningPathQueryHandler : IRequestHandler<GetContentByLearningPathQuery, Result<List<ContentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentByLearningPathQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<ContentDto>>> Handle(GetContentByLearningPathQuery request, CancellationToken cancellationToken)
    {
        var contents = await _unitOfWork.Contents.GetContentByLearningPathAsync(request.LearningPathId);
        var dto = _mapper.Map<List<ContentDto>>(contents);
        return Result<List<ContentDto>>.Success(dto);
    }
}
/*
public class CompleteContentCommandHandler : IRequestHandler<MarkContentAsCompletedCommand, Result<bool>>
{
    public IUnitOfWork _unitOfWork { get; }

    public CompleteContentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(MarkContentAsCompletedCommand request, CancellationToken cancellationToken)
    {
            var content = await _unitOfWork.Contents.FindAsync(request.ContentId);
        if (content == null)
        {
            throw new NotFoundException(nameof(Content), request.ContentId);
        }

        var user = await _unitOfWork.Users.FindAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var progress = await _context.ContentProgress
            .FirstOrDefaultAsync(p => p.ContentId == request.ContentId && p.UserId == request.UserId, cancellationToken);

        if (progress == null)
        {
            progress = new ContentProgress
            {
                ContentId = request.ContentId,
                UserId = request.UserId,
                IsCompleted = true,
                TimeSpentMinutes = request.TimeSpentMinutes,
                LastAccessed = DateTime.UtcNow
            };
            _context.ContentProgress.Add(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.TimeSpentMinutes = request.TimeSpentMinutes;
            progress.LastAccessed = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
*/

public class CompleteContentCommandHandler : IRequestHandler<CompleteContentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CompleteContentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CompleteContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(request.ContentId);
        if (content == null)
        {
            throw new NotFoundException(nameof(Content), request.ContentId);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var progress = await _unitOfWork.UserProgress
            .SingleOrDefaultAsync(p => p.ContentId == request.ContentId && p.UserId == request.UserId);

        if (progress == null)
        {
            progress = new UserProgress
            {
                ContentId = request.ContentId,
                UserId = request.UserId,
                IsCompleted = true,
                TimeSpentMinutes = request.TimeSpentMinutes,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.UserProgress.AddAsync(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.TimeSpentMinutes = request.TimeSpentMinutes;
            progress.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserProgress.Update(progress);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}

public class GetContentProgressQueryHandler : IRequestHandler<GetContentProgressQuery, Result<ContentProgressDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentProgressDto>> Handle(GetContentProgressQuery request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(request.ContentId);
        if (content == null)
        {
            throw new NotFoundException(nameof(Content), request.ContentId);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var progress = await _unitOfWork.UserProgress
            .SingleOrDefaultAsync(p => p.ContentId == request.ContentId && p.UserId == request.UserId);

        var progressDto = _mapper.Map<ContentProgressDto>(progress);

        /*if (progress == null)
        {
            return Result<ContentProgressDto>.Success(progressDto);
        }*/
        return Result<ContentProgressDto>.Success(progressDto);
    }
}

public class GetNextContentQueryHandler : IRequestHandler<GetNextContentQuery, Result<ContentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNextContentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentDto>> Handle(GetNextContentQuery request, CancellationToken cancellationToken)
    {
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            throw new NotFoundException(nameof(LearningPath), request.LearningPathId);
        }

        var currentContent = await _unitOfWork.Contents.GetByIdAsync(request.CurrentContentId);
        if (currentContent == null)
        {
            throw new NotFoundException(nameof(Content), request.CurrentContentId);
        }

        var nextContent = await _unitOfWork.Contents.GetNextContentAsync(request.CurrentContentId, request.LearningPathId);

        if (nextContent == null)
        {
            return Result<ContentDto>.Success(null); // No next content
        }

        var contentDto = _mapper.Map<ContentDto>(nextContent);
        return Result<ContentDto>.Success(contentDto);
    }
}

public class GetPreviousContentQueryHandler : IRequestHandler<GetPreviousContentQuery, Result<ContentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPreviousContentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentDto>> Handle(GetPreviousContentQuery request, CancellationToken cancellationToken)
    {
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            throw new NotFoundException(nameof(LearningPath), request.LearningPathId);
        }

        var currentContent = await _unitOfWork.Contents.GetByIdAsync(request.CurrentContentId);
        if (currentContent == null)
        {
            throw new NotFoundException(nameof(Content), request.CurrentContentId);
        }

        var previousContent = await _unitOfWork.Contents.GetPreviousContentAsync(request.CurrentContentId, request.LearningPathId);

        if (previousContent == null)
        {
            return Result<ContentDto>.Success(null); // No previous content
        }

        var contentDto = _mapper.Map<ContentDto>(previousContent);
        return Result<ContentDto>.Success(contentDto);
    }
}

