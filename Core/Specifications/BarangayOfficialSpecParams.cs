namespace Core.Specifications;

public class BarangayOfficialSpecParams : PagingParams
{
    public string? Sort { get; set; }
    private string? _search;
    public string Search 
    { 
        get => _search ?? ""; 
        set => _search = value?.Trim().ToLower(); 
    }
}
