using System.Collections.Frozen;
using System.Collections.Immutable;

namespace RTSCore.Domain.Configurations.Factions;

public sealed record FactionCatalog(ImmutableArray<FactionTemplate> Templates)
{
    private readonly FrozenDictionary<FactionType, FactionTemplate> _factions = Templates.ToFrozenDictionary(t => t.Type);

    public FactionTemplate GetFaction(FactionType factionType) => _factions[factionType];

}