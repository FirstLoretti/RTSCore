using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Armies.Combat;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Diplomacy;
using RTSCore.Domain.Configurations.Factions;

namespace RTSCore.Domain.Configurations;

public class GameConfiguations
{
    public UnitCatalog Units { get; init; } = new([]);
    public FactionCatalog Factions { get; init; } = new([]);
    public BuildingCatalog Buildings { get; init; } = new([]);
    public CityCatalog Cities { get; init; } = new([]);
    public AiCatalog AiPersotalities { get; init; } = new([]);

    public MovementConfiguration Movement { get; init; } = new();
    public DiplomacyConfiguration Diplomacy { get; init; } = new();
    public AutoBattleConfiguration AutoBattle { get; init; } = new();
    public UnitConfiguration UnitConfiguration { get; init; } = new([], new());
}