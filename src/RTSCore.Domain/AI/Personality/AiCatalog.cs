using System.Collections.Frozen;

namespace RTSCore.Domain.AI.Personality;

public record AiCatalog
{
    private readonly FrozenDictionary<AiPersonalityType, AiPersonality> _personalities;

    public AiPersonality GetPersonality(AiPersonalityType personalityType) => _personalities[personalityType];

    public AiCatalog(IReadOnlyCollection<AiPersonality> personalities)
    {
        _personalities = personalities.ToFrozenDictionary(p => p.Type);
    }
}