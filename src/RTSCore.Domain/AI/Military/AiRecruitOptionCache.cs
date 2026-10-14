using System.Collections.Immutable;

using RTSCore.Application.AI.Brains;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;

namespace RTSCore.Domain.AI.Military;

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