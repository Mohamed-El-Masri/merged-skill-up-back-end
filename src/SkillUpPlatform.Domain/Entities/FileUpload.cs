using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class FileUpload : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsPublic { get; set; }
    public string? Description { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<FileShare> FileShares { get; set; } = new List<FileShare>();
}

public class FileShare : BaseEntity
{
    public int FileUploadId { get; set; }
    public int SharedWithUserId { get; set; }
    public DateTime SharedAt { get; set; }
    public int SharedByUserId { get; set; }
    public string? AccessLevel { get; set; } = "Read"; // Read, Write, Admin
    public virtual FileUpload FileUpload { get; set; } = null!;
    public virtual User SharedWithUser { get; set; } = null!;
    public virtual User SharedByUser { get; set; } = null!;
}
