namespace RTSCore.Domain.Entities.Armies.Combat;

public interface IAutoBattleService
{
    BattleResult StartAutoBattle(Army attacker, Army defender);
}