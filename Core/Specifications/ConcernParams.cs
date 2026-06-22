using Core.Enums;

namespace Core.Specifications;
public class ConcernParams : PagingParams
{
    public ConcernPriority? Priority { get; set; }
    public string? Sort { get; set; } = "daseDesc";
    private string? _search;
    public string Search 
    { 
        get => _search ?? "";
        set => _search = value?.Trim().ToLower();
        }
}