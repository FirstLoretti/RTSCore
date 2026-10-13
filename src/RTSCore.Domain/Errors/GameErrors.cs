namespace RTSCore.Domain.Errors;

public static class GameErrors
{
    public static GameError EntityNotFound(Guid id) =>
        new("ENTITY_NOT_FOUND", $"Сущность с id:{id} не найдена");

    public static GameError EntityNotFound<TEnum>(TEnum type) where TEnum : Enum =>
        new("ENTITY_NOT_FOUND", $"Сущность с типом:{type} не найдена");


    public static GameError CannotTargetSelf =>
        new("TARGET_SELF", "Нельзя выбрать целью самого себя");
}