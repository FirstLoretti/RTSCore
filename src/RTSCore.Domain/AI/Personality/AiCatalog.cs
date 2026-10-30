using System.Collections.Frozen;
using System.Collections.Immutable;

namespace RTSCore.Domain.AI.Personality;

public sealed record AiCatalog
{
    public readonly ImmutableArray<AiPersonality> Personalities;
    private readonly FrozenDictionary<AiPersonalityType, AiPersonality> _personalities;

    public AiPersonality GetPersonality(AiPersonalityType personalityType) => _personalities[personalityType];

    public AiCatalog(IReadOnlyCollection<AiPersonality> personalities)
    {
        Personalities = [.. personalities];

        var tempDic = new Dictionary<AiPersonalityType, AiPersonality>(personalities.Count);
        foreach (var personality in Personalities)
        {
            tempDic[personality.Type] = personality;
        }
        _personalities = tempDic.ToFrozenDictionary();
    }
}