using System.Collections.Frozen;
using System.Collections.Immutable;

namespace RTSCore.Domain.Configurations.Factions;

public sealed class FactionCatalog(IReadOnlyList<FactionTemplate> templates)
{
    private readonly FrozenDictionary<FactionType, FactionTemplate> _factions = templates.ToFrozenDictionary(t => t.Type);

    public ImmutableArray<FactionTemplate> AllTemplates => _factions.Values;
    public FactionTemplate GetFaction(FactionType factionType) => _factions[factionType];

}