using System.Collections.Immutable;

using RTSCore.Application.AI.Brains;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Application.AI.Recruitment;

public class AiRecruitOptionCache
{
    private readonly Dictionary<AiPersonality, ImmutableArray<AiRecruitOption>> _cache = [];

    public AiRecruitOptionCache(
        IReadOnlyCollection<AiPersonality> personalities,
        IReadOnlyCollection<UnitTemplate> templates,
        UnitUtilityCalculator utilityCalculator
    )
    {
        foreach (var personality in personalities)
        {
            var options = utilityCalculator.CalculateFor(templates, personality);
            _cache[personality] = [.. options];
        }
    }

    public ImmutableArray<AiRecruitOption> GetOptionFor(AiPersonality aiPersonality)
    {
        return _cache.TryGetValue(aiPersonality, out var options)
            ? options
            : [];
    }
}