using Core.Entities;

namespace Core.Specifications;

public class AnnouncementSpecification : BaseSpecification<Announcement>
{
    public AnnouncementSpecification(AnnouncementSpecParams specParams) : 
        base(x =>
            string.IsNullOrEmpty(specParams.Search) ||
            x.Title.ToLower().Contains(specParams.Search) ||
            x.Message.ToLower().Contains(specParams.Search)
        )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        
        switch (specParams.Sort)
        {
            case "dateAsc":
                AddOrderBy(x => x.CreateAt);
                break;
            
            case "titleAsc":
                AddOrderBy(x => x.Title);
                break;

            default:
                AddOrderByDescending(x => x.CreateAt);
                break;
        }
    }

    public AnnouncementSpecification()
    {
    }
}
