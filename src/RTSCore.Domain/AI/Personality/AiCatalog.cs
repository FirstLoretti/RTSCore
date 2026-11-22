using System.Collections.Frozen;
using System.Collections.Immutable;

namespace RTSCore.Domain.AI.Personality;

public sealed class AiCatalog(IReadOnlyCollection<AiPersonality> personalities)
{
    private readonly FrozenDictionary<AiPersonalityType, AiPersonality> _personalities =
        personalities.ToFrozenDictionary(p => p.Type);

    public ImmutableArray<AiPersonality> AllPersonalities => _personalities.Values;
    public AiPersonality GetPersonality(AiPersonalityType personalityType) => _personalities[personalityType];
}