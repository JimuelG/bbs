using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class LatestAnnouncementSpecification : BaseSpecification<Announcement>
{
    public LatestAnnouncementSpecification(DateTime currentTime) : base(a => a.IsActive && !a.IsPlayed && a.ScheduledAt <= currentTime)
    {
        AddOrderByDescending(a => a.Id);
    }

}
