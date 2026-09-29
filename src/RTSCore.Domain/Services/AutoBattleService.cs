using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

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