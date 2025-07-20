using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Users.DTOs;
using SkillUpPlatform.Application.Features.Users.Queries;
using SkillUpPlatform.Domain.Interfaces;
using System.Text.Json;

namespace SkillUpPlatform.Application.Features.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<Common.Models.UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Common.Models.UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result<Common.Models.UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        var userDto = _mapper.Map<Common.Models.UserDto>(user);
        return Result<Common.Models.UserDto>.Success(userDto);
    }
}

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserWithProfileAsync(request.UserId);
        
        if (user == null)
        {
            return Result<UserProfileDto>.Failure(ErrorMessages.UserNotFound);
        }

        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Bio = user.UserProfile?.Bio,
            LinkedInUrl = user.UserProfile?.LinkedInUrl,
            GitHubUrl = user.UserProfile?.GitHubUrl,
            PortfolioUrl = user.UserProfile?.PortfolioUrl,
            ProfilePictureUrl = user.UserProfile?.ProfilePictureUrl,
            Skills = !string.IsNullOrEmpty(user.UserProfile?.Skills) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Skills) ?? new List<string>()
                : new List<string>(),
            Interests = !string.IsNullOrEmpty(user.UserProfile?.Interests) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Interests) ?? new List<string>()
                : new List<string>(),
            Certifications = !string.IsNullOrEmpty(user.UserProfile?.Certifications) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Certifications) ?? new List<string>()
                : new List<string>()
        };

        return Result<UserProfileDto>.Success(profileDto);
    }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<Common.Models.UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<Common.Models.UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        
        // Apply search filter if provided
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            users = users.Where(u => 
                u.FirstName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Apply pagination
        users = users.Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize);

        var userDtos = _mapper.Map<List<Common.Models.UserDto>>(users.ToList());
        return Result<List<Common.Models.UserDto>>.Success(userDtos);
    }
}
