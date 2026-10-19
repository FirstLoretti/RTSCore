using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Armies.Combat;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Diplomacy;
using RTSCore.Domain.Configurations.Factions;

namespace RTSCore.Domain.Configurations;

public sealed record GameConfigurations
{
    public required UnitCatalog Units { get; init; }
    public required FactionCatalog Factions { get; init; }
    public required BuildingCatalog Buildings { get; init; }
    public required CityCatalog Cities { get; init; }
    public required AiCatalog AiPersonalities { get; init; }

    public required MovementConfiguration Movement { get; init; }
    public required DiplomacyConfiguration Diplomacy { get; init; }
    public required AutoBattleConfiguration AutoBattle { get; init; }
    public required UnitConfiguration UnitConfiguration { get; init; }
    public required CityConfiguration CityConfiguration { get; init; }
}