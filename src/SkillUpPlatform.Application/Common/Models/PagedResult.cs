namespace SkillUpPlatform.Application.Common.Models;

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasNextPage => TotalCount > 0 && Page < TotalPages;
    public bool HasPreviousPage => TotalCount > 0 && Page > 1;

    public PagedResult()
    {
    }

    public PagedResult(List<T> data, int totalCount, int page, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
