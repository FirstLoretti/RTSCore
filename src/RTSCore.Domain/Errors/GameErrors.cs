namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError NotEnoughMovementPoints { get; }
        = new("Army.NotEnoughMovementPoints", "Not enough movement points.");
    public static GameError ArmyDoesNotHaveFreeSlots { get; }
        = new("Army.DoesNotHaveFreeSlots", "Army does not have free slots.");
    public static GameError CantCancelRecruitUnit { get; }
        = new("Army.CantCancelRecruitUnit", "Cannot cancel recruitment for an already recruited unit.");
    public static GameError CantDisbandGeneral { get; }
        = new("Army.CantDisbandGeneral", "General cannot be disbanded.");

    public static GameError UnavailableRecruitOption { get; }
        = new("City.UnavailableRecruitOption", "Recruit option is unavailable.");

    public static GameError CityUnavailableConstructionOption { get; }
        = new("City.UnavailableConstructionOption", "Unavailable construction option for this city type.");
    public static GameError CantConstructBuildingDuplicate { get; }
        = new("City.CantConstructBuildingDuplicate", "Cannot construct a duplicate building.");
    public static GameError ChainUnavailableConstructionOption { get; }
        = new("City.ChainUnavailableConstructionOption", "Building chain conditions are not met.");
    public static GameError CantCancelAlreadyConstructed { get; }
        = new("City.CantCancelAlreadyConstructed", "Cannot cancel an already constructed building.");

    public static GameError CantTradeWithEnemy { get; }
        = new("Diplomacy.CantTradeWithEnemy", "Cannot trade with an enemy faction.");
    public static GameError CantTradeAlreadyExist { get; }
        = new("Diplomacy.CantTradeAlreadyExist", "Trade agreement already exists.");
    public static GameError CantWarFactionsAlreadyWar { get; }
        = new("Diplomacy.CantWarFactionsAlreadyWar", "Factions are already at war.");
    public static GameError CantPeaceFactionsNotWar { get; }
        = new("Diplomacy.CantPeaceFactionsNotWar", "Factions are not at war.");
    public static GameError CantCancelTradeNotExist { get; }
        = new("Diplomacy.CantCancelTradeNotExist", "Trade agreement does not exist.");
}