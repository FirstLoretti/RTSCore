using System.Collections.Frozen;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.Configurations.Armies;

public record UnitCatalog(IReadOnlyCollection<UnitTemplate> Templates)
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _units = Templates.ToFrozenDictionary(t => t.Type);

    public UnitTemplate Get(UnitType unitType) => _units[unitType];
}