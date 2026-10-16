namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError CantTargetSelf => new("Can't target self.");
    public static GameError NotEnoughMovementPoints => new("Not enough movement points.");
    public static GameError ArmyDontHaveFreeSlots => new("Army don't have free slots.");
    public static GameError CantCancelRecruitUnit => new("Can't cancel recruit already recruited unit.");
    public static GameError CantDisbandGeneral => new("Army general can't be disbanded.");
}