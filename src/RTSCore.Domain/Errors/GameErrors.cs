namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError CantTargetSelf => new("Cannot target self.");
    public static GameError NotEnoughMovementPoints => new("Not enough movement points.");
    public static GameError ArmyDoesNotHaveFreeSlots => new("Army does not have free slots.");
    public static GameError CantDisbandGeneral => new("General cannot be disbanded.");

    public static GameError UnavailableRecruitOption => new("Recruit option is unavailable.");
    public static GameError CantCancelRecruitUnit => new("Cannot cancel recruitment for an already recruited unit.");
    public static GameError CityUnavailableConstructionOption => new("Unavailable construction option for this city type.");
    public static GameError CantConstructBuildingDuplicate => new("Cannot construct a duplicate building.");
    public static GameError ChainUnavailableConstructionOption => new("Building chain conditions are not met.");
    public static GameError CantCancelAlreadyConstructed => new("Cannot cancel an already constructed building.");

    public static GameError CantTradeWithEnemy => new("Cannot trade with an enemy faction.");
    public static GameError CantTradeAlreadyExist => new("Trade agreement already exists.");
    public static GameError CantWarFactionsAlreadyWar => new("Factions are already at war.");
    public static GameError CantPeaceFactionsNotWar => new("Factions are not at war.");
    public static GameError CantCancelTradeNotExist => new("Trade agreement does not exist.");
}