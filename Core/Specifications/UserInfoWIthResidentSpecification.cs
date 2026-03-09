using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications;

public class UserInfoWIthResidentSpecification : BaseSpecification<AppUser>
{
    public UserInfoWIthResidentSpecification(string id) : base(a => a.Id == id)
    {
        AddInclude(x => x.Resident!);
    }

    protected UserInfoWIthResidentSpecification()
    {
    }
}
