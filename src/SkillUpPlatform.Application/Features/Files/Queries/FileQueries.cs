using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Files.Commands;

namespace SkillUpPlatform.Application.Features.Files.Queries;

public class GetUserFilesQuery : IRequest<Result<PagedResult<FileInfoDto>>>
{
    public int UserId { get; set; }
    public bool? IsPublic { get; set; }
    public string? FileType { get; set; }
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetFileByIdQuery : IRequest<Result<FileUploadDto>>
{
    public int FileId { get; set; }
    public int UserId { get; set; }
}

public class GetPublicFilesQuery : IRequest<Result<List<FileUploadDto>>>
{
    public string? FileType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetSharedFilesQuery : IRequest<Result<List<FileUploadDto>>>
{
    public int UserId { get; set; }
    public string? AccessLevel { get; set; }
}

public class DownloadFileQuery : IRequest<Result<FileDownloadDto>>
{
    public int FileId { get; set; }
    public int UserId { get; set; }
}

public class FileDownloadDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = new byte[0];
    public long FileSize { get; set; }
}

public class GetFileDownloadQuery : IRequest<Result<FileDownloadDto>>
{
    public int FileId { get; set; }
    public int UserId { get; set; }
}

public class GetFileDetailsQuery : IRequest<Result<FileDetailsDto>>
{
    public int FileId { get; set; }
    public int UserId { get; set; }
}

public class GetFileCategoriesQuery : IRequest<Result<List<FileCategoryDto>>>
{
    public int UserId { get; set; }
}

public class FileCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int FileCount { get; set; }
}

public class GetFileUsageStatisticsQuery : IRequest<FileUsageStatisticsDto>
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class FileUsageStatisticsDto
{
    public long TotalFiles { get; set; }
    public long TotalSize { get; set; }
    public long TotalDownloads { get; set; }
    public Dictionary<string, int> FileTypeDistribution { get; set; } = new();
    public Dictionary<string, long> CategorySizes { get; set; } = new();
    public List<PopularFileDto> PopularFiles { get; set; } = new();
}

public class PopularFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DownloadCount { get; set; }
    public DateTime LastDownloaded { get; set; }
}
