using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class LatestAnnouncementSpecification : BaseSpecification<Announcement>
{
    public LatestAnnouncementSpecification() : base(a => a.IsActive && !a.IsPlayed && a.ScheduledAt <= DateTime.UtcNow)
    {
        AddOrderByDescending(a => a.Id);
    }

}
