namespace Core.Specifications;

public class AnnouncementSpecParams :  PagingParams
{
    public string? Sort { get; set; }
    private string? _search;
    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }
}
