namespace RTSCore.Domain.Entities.Armies.Combat;

public class AutoBattleService(IAutoBattleCalculator calculator)
{
    public BattleResult StartAutoBattle(Army attacker, Army defender)
    {
        var battleResult = calculator.Calculate(attacker.Units, defender.Units);

        attacker.TakeCasualties(battleResult.AttackerBattleLogs);
        defender.TakeCasualties(battleResult.DefenderBattleLogs);

        return battleResult;
    }
}