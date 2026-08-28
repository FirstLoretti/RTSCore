namespace RTSCore.Domain.Exeptions;

public class GameRuleException(string message) : Exception(message);