namespace Core.Entities;

public class RPiStatus : BaseEntity
{
    public DateTime LastSeen { get; set; }
    public bool IsOnline => (DateTime.UtcNow - LastSeen).TotalSeconds < 90;
}
