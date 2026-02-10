using System;

namespace Core.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime UpdatedAt { get; set; }
}
