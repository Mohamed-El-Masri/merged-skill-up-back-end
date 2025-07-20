using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Interfaces;

public class PerformanceService : IPerformanceService
{
    public Task<PerformanceMetricsDto> GetMetricsAsync(string period)
    {
        var dto = new PerformanceMetricsDto
        {
            AverageResponseTime = 120.5,
            TotalRequests = 10000,
            ErrorCount = 150,
            ErrorRate = 1.5,
            Uptime = 99.95,
            EndpointMetrics = new Dictionary<string, double>
            {
                { "/api/users", 110.2 },
                { "/api/courses", 130.8 },
                { "/api/login", 90.6 }
            }
        };

        return Task.FromResult(dto);
    }
}
