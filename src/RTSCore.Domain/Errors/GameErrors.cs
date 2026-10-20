namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError CantTargetSelf => new("Can't target self.");
    public static GameError NotEnoughMovementPoints => new("Not enough movement points.");
    public static GameError ArmyDontHaveFreeSlots => new("Army don't have free slots.");
    public static GameError CantDisbandGeneral => new("Army general can't be disbanded.");

    public static GameError UnavailableRecruitOption => new("Unavailable recruit option.");
    public static GameError CantCancelRecruitUnit => new("Can't cancel recruit already recruited unit.");
    public static GameError CityUnavailableConstructionOption => new("Unavailable construction option for this city type.");
    public static GameError CantConstructBuildingDublicate => new("Can't construct building dublicate.");
    public static GameError ChainUnavailableConstructionOption => new("Don't meet all chain conditions.");
    public static GameError CantCancelAlreadyConstructed => new("Can't cancel already constructed building.");

    public static GameError CantTradeWithWarOpponent => new("Can't trade with war opponent.");
    public static GameError CantTradeAlreadyConcluded => new("Trade agreement has already concluded.");
    public static GameError CantWarAlreadyHappening => new("War is already happening.");
    public static GameError CantPeaceFactionsNotAtWarState => new("Factions not at war state.");
    public static GameError CantCancelTradeDontConcluded => new("Trade don't concluded");
}