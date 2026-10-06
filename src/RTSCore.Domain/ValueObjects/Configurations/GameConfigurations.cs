using RTSCore.Domain.Entities.Campaign;

namespace RTSCore.Domain.ValueObjects.Configurations;

public class GameConfigurations
{
    public UnitConfiguration Units { get; set; } = new([], [], new());
    public MovementConfiguration Movement { get; set; } = new();
    public CityConfiguration Cities { get; set; } = new([]);
    public DiplomacyConfiguration Diplomacy { get; set; } = new();
    public FactionConfiguration Factions { get; set; } = new([]);
    public BuildingConfiguration Buildings { get; set; } = new([]);
    public AutoBattleConfiguration AutoBattle { get; set; } = new();
}