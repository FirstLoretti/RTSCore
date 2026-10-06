using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects.Results;

namespace RTSCore.Domain.Services.Combat;

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