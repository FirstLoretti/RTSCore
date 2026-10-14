namespace RTSCore.Domain.Entities.Armies.Combat;

public interface IAutoBattleCalculator
{
    BattleResult Calculate(IReadOnlyCollection<Unit> attakers, IReadOnlyCollection<Unit> defenders);
}