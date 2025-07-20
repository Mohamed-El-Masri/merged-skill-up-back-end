using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class UserProfile : BaseEntity
{
    public int UserId { get; set; }
    public string? Bio { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Skills { get; set; } // JSON string of skills array
    public string? Interests { get; set; } // JSON string of interests array
    public string? Certifications { get; set; } // JSON string of certifications array

    // Navigation Properties
    public virtual User User { get; set; } = null!;
}
