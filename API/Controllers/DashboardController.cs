using API.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class DashboardController(
    IUnitOfWork unit,
    UserManager<AppUser> userManager
) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var residentUsers = await userManager.Users
            .Include(u => u.Resident)
            .Where(u => u.Resident != null && !u.Resident.IsDeleted)
            .ToListAsync();

        var residents = residentUsers
            .Select(u => u.Resident!)
            .ToList();

        var certificates = await unit.Repository<BarangayCertificate>().ListAllAsync();
        var concerns = await unit.Repository<Concern>().ListAllAsync();
        var announcements = await unit.Repository<Announcement>().ListAllAsync();
        var officials = await unit.Repository<BarangayOfficial>().ListAllAsync();

        var verifiedResidents = await userManager.Users
            .CountAsync(x => x.IsIdVerified);

        var residentLookup = residents.ToDictionary(x => x.Id, x => x);

        var pendingCertificates = certificates
            .Count(x => x.Status == CertificateStatus.Pending);

        var activeConcerns = concerns.Count(x =>
            x.Status.ToString() != "Resolved" &&
            x.Status.ToString() != "Closed"
        );

        var activeAnnouncements = announcements.Count(x => !x.IsPlayed);

        var recentRequest = certificates
            .OrderByDescending(x => x.RequestDate)
            .Take(5)
            .Select(x => new RecentCertificateRequestDto
            {
                Id = x.Id,
                Name = x.FullName,
                Type = x.CertificateType.ToString(),
                Date = x.RequestDate,
                Status = x.Status
            })
            .ToList();

        var urgentConcerns = concerns
            .Where(x => 
                x.Status.ToString() != "Resolved" &&
                x.Status.ToString() != "Closed"
            )
            .OrderByDescending(x => x.DateReported)
            .Take(5)
            .Select(x =>
            {
                residentLookup.TryGetValue(x.ResidentId, out var resident);

                return new UrgentConcernDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Purok = resident?.Purok ?? "N/A",
                    Priority = x.Priority.ToString()
                };
            })
            .ToList();

        var dashboardOfficials = officials
            .OrderBy(x => x.Id)
            .Take(6)
            .Select(x => new DashboardOfficialDto
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName,
                Position = x.Position,
                ImageUrl = x.OfficeImage
            })
            .ToList();

        var dashboard = new DashboardDto
        {
            Stats = new DashboardStatsDto
            {
                TotalResidents = residents.Count,
                VerifiedResidents = verifiedResidents,
                PendingCertificates = pendingCertificates,
                ActiveConcerns = activeConcerns,
                ActiveAnnouncements = activeAnnouncements
            },
            RecentRequest = recentRequest,
            RecentConcern = urgentConcerns,
            Officials = dashboardOfficials
        };

        return Ok(dashboard);
    }
    
}
