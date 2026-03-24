using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class AnnouncementManualTriggerSpecification : BaseSpecification<Announcement>
{
    public AnnouncementManualTriggerSpecification() : base(x => x.ManualTriggerActive == true)
    {
        AddOrderByDescending(x => x.ScheduledAt);
    }

    public AnnouncementManualTriggerSpecification(int id) : base(x => x.Id == id)
    {
    }
}
