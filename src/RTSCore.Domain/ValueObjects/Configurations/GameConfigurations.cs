namespace RTSCore.Domain.ValueObjects.Configurations;

public class GameConfigurations
{
    public UnitConfiguration Units { get; set; } = new([], [], new());
    public ArmyConfiguration Army { get; set; } = new();
    public CityConfiguration Cities { get; set; } = new([]);
    public DiplomacyConfiguration Diplomacy { get; set; } = new();
}