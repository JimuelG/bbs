using Core.Enums;

namespace Core.Interfaces;

public interface IConcernPriorityService
{
    ConcernPriority DeterminePriority(
        ConcernType type,
        string title,
        string description
    );
}
