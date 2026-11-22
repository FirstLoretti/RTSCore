using System.Collections.Frozen;
using System.Collections.Immutable;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.Configurations.Armies;

public sealed class UnitCatalog(IReadOnlyList<UnitTemplate> templates)
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _units = templates.ToFrozenDictionary(t => t.Type);

    public ImmutableArray<UnitTemplate> AllTemplates => _units.Values;
    public UnitTemplate GetTemplate(UnitType unitType) => _units[unitType];
}