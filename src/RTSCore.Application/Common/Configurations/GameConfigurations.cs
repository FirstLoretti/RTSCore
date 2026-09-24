using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Common.Configurations;

public class GameConfigurations
{
    public UnitConfiguration Units { get; set; } = new([], [], new());
    public ArmyConfiguration Army { get; set; } = new();
    public CityConfiguration Cities { get; set; } = new([]);
}