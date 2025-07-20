using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Contentt.Commands;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Application.Features.Contentt.Queries;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Features.Contentt.Handlers;

public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateContentCommand request, CancellationToken cancellationToken)
    {
        var contentt = _mapper.Map<Content>(request);

        await _unitOfWork.Contents.AddAsync(contentt);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(contentt.Id);
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
