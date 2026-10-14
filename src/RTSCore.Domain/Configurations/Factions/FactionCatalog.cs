using System.Collections.Frozen;

namespace RTSCore.Domain.Configurations.Factions;

public record FactionCatalog(IReadOnlyCollection<FactionTemplate> Templates)
{
    private readonly FrozenDictionary<FactionType, FactionTemplate> _factions = Templates.ToFrozenDictionary(t => t.Type);

    public FactionTemplate Get(FactionType factionType) => _factions[factionType];

}