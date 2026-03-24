using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class AnnouncementSpecification : BaseSpecification<Announcement>
{
    public AnnouncementSpecification(AnnouncementSpecParams specParams) : 
        base()
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        
    }

    protected AnnouncementSpecification()
    {
    }
}
