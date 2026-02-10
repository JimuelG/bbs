using Core.Entities;

namespace Core.Interfaces;

public interface IAnnouncementRepository
{
    Task<Announcement?> GetLatestAsync();
    Task<Announcement?> GetById(int id);
    Task<IReadOnlyList<Announcement>> GetActiveAsync();
    Task AddAsync(Announcement announcement);
    void Update(Announcement announcement);
}
