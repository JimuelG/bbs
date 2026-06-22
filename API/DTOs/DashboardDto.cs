namespace API.DTOs;

public class DashboardDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<RecentCertificateRequestDto> RecentRequest { get; set; } = [];
    public List<UrgentConcernDto> RecentConcern { get; set; } = [];
    public List<DashboardOfficialDto> Officials { get; set; } = [];
}

public class DashboardStatsDto
{
    public int TotalResidents { get; set; }
    public int VerifiedResidents { get; set; }
    public int PendingCertificates { get; set; }
    public int ActiveConcerns { get; set; }
    public int ActiveAnnouncements { get; set; }
}

public class RecentCertificateRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UrgentConcernDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public class DashboardOfficialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

