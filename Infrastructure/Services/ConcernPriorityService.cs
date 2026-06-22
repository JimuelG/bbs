using Core.Enums;
using Core.Interfaces;

namespace Infrastructure.Services;

public class ConcernPriorityService : IConcernPriorityService
{
    public ConcernPriority DeterminePriority(ConcernType type, string title, string description)
    {
        var typeText = type.ToString().ToLowerInvariant();
        var text = $"{title} {description}".ToLowerInvariant();

        var p1Keywords = new[]
        {
            "fire", "sunog",
            "accident", "aksidente",
            "injured", "nasugatan", "sugat",
            "medical", "ambulance", "emergency",
            "violence", "away", "riot",
            "crime", "nakawan", "theft", "robbery",
            "flood", "baha",
            "live wire", "kuryente", "spark",
            "gas leak",
            "landslide",
            "collapsed", "gumuho"
        };

        if (p1Keywords.Any(keyword => text.Contains(keyword)))
        {
            return ConcernPriority.P1;
        }

        if (
            typeText.Contains("emergency") ||
            typeText.Contains("disaster") ||
            typeText.Contains("crime") ||
            typeText.Contains("health") ||
            typeText.Contains("peace")
        )
        {
            return ConcernPriority.P1;
        }

        var p2Keywords = new[]
        {
            "garbage", "basura",
            "drainage", "barado", "clogged",
            "streetlight", "ilaw",
            "road", "kalsada", "pothole",
            "water", "tubig",
            "noise", "ingay",
            "stray dog", "aso",
            "sanitation",
            "broken", "sira"
        };

        if (p2Keywords.Any(keyword => text.Contains(keyword)))
        {
            return ConcernPriority.P2;
        }

        if (
            typeText.Contains("sanitation") ||
            typeText.Contains("infrastructure") ||
            typeText.Contains("road") ||
            typeText.Contains("utilities") ||
            typeText.Contains("environment")
        )
        {
            return ConcernPriority.P2;
        }

        return ConcernPriority.P3;
    }
}