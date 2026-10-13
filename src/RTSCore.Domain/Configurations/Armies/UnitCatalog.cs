using System.Collections.Frozen;

using RTSCore.Domain.Enums;

namespace RTSCore.Domain.Configurations.Units;

public record UnitCatalog(IReadOnlyCollection<UnitTemplate> Templates)
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _units = Templates.ToFrozenDictionary(t => t.Type);

    public UnitTemplate Get(UnitType unitType) => _units[unitType];
}