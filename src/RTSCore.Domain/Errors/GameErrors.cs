using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static Error NotEnoughMovementPoints { get; }
        = new("Army.NotEnoughMovementPoints", "Not enough movement points.");
    public static Error ArmyDoesNotHaveFreeSlots { get; }
        = new("Army.DoesNotHaveFreeSlots", "Army does not have free slots.");
    public static Error CantDisbandGeneral { get; }
        = new("Army.CantDisbandGeneral", "General cannot be disbanded.");
    public static Error CantMoveWithoutGeneral { get; }
        = new("Army.CantMoveWithoutGeneral", "Army cannot move without general.");
    public static Error UnitNotInTargetArmy(UnitId unitId, ArmyId armyId) =>
        new("Army.UnitNotInThisArmy", $"Unit: {unitId} is not exist in army: {armyId}.");
    public static Error CantCancelRecruitmentUnitUnsuitableCondition(bool isAlive, bool isRecruited) =>
        new("Army.CantCancelRecruitmentUnitUnsuitableCondition",
            $"Cannot cancel recruitment, unit in unsuitable condition isAlive: {isAlive}, isRecruited {isRecruited}.");

    public static Error CantConstructThisBuildingType { get; }
        = new("City.CityCantConstructThisBuildingType", "This building do not allowed for construction in this city type.");
    public static Error CantRecruitBuildingNotConstructed { get; }
        = new("City.CityCantRecruitBuildingNotConstructed", "Cannot recruit unit, construct required building first.");
    public static Error CantConstructBuildingDuplicate { get; }
        = new("City.CantConstructBuildingDuplicate", "Cannot construct a duplicate building.");
    public static Error ChainUnavailableConstructionOption { get; }
        = new("City.ChainUnavailableConstructionOption", "Building chain conditions are not met.");
    public static Error CantCancelAlreadyConstructed { get; }
        = new("City.CantCancelAlreadyConstructed", "Cannot cancel an already constructed building.");
    public static Error BuildingNotInTargetCity(BuildingId buildingId, CityId cityId) =>
        new("City.BuildingNotInTargetCity", $"Building: {buildingId} is not exist in city: {cityId}.");

    public static Error CantTradeWithEnemy { get; }
        = new("Diplomacy.CantTradeWithEnemy", "Cannot trade with an enemy faction.");
    public static Error CantTradeAlreadyExist { get; }
        = new("Diplomacy.CantTradeAlreadyExist", "Trade agreement already exists.");
    public static Error CantWarFactionsAlreadyWar { get; }
        = new("Diplomacy.CantWarFactionsAlreadyWar", "Factions are already at war.");
    public static Error CantPeaceFactionsNotWar { get; }
        = new("Diplomacy.CantPeaceFactionsNotWar", "Factions are not at war.");
    public static Error CantCancelTradeNotExist { get; }
        = new("Diplomacy.CantCancelTradeNotExist", "Trade agreement does not exist.");
    public static Error InvalidOfferStatus { get; }
        = new("Diplomacy.InvalidOfferStatus", "Cannot acccept or reject offer with not pending status.");
    public static Error RelationNotContainFactions(FactionType initiator, FactionType target, DiplomacyRelationId id) =>
        new("Diplomacy.RelationNotContainFactions",
            $"Initiator: {initiator} or target: {target} is not part of relation: {id}");

    public static Error NotEnoughGold { get; } = new("NotEnoughGold", "Not enough gold");
}