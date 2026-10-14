namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError CannotTargetSelf => new("TARGET_SELF", "Нельзя выбрать целью самого себя");
}