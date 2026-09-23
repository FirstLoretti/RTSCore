namespace RTSCore.Application.Common.Configurations;

public class GameConfigurations
{
    public UnitConfiguration Units { get; init; } = new([], []);
    public ArmyConfiguration Army { get; init; } = new();
}