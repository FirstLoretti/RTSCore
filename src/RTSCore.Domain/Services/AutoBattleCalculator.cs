using System.Collections.Frozen;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.Services;

public class AutoBattleCalculator(
    IReadOnlyCollection<UnitTemplate> units,
    AutoBattleConfiguration configuration
) : IAutoBattleCalculator
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _typeToTemplate = units.ToFrozenDictionary(u => u.Type);

    public BattleResult Calculate(IReadOnlyCollection<Unit> attakers, IReadOnlyCollection<Unit> defenders)
    {
        var liveAttackers = attakers.Where(u => u.IsAlive).ToList();
        var attackerPower = liveAttackers
            .Sum(u =>
            {
                var template = _typeToTemplate.GetValueOrDefault(u.Type);
                return template == null
                    ? throw new NullReferenceException("Шаблона не существует")
                    : UnitStatsCalculator.CalculateCurrentPower(template, u.Level, u.Health);
            });

        var liveDifenders = defenders.Where(u => u.IsAlive).ToList();
        var defenderPower = liveDifenders
            .Sum(u =>
            {
                var template = _typeToTemplate.GetValueOrDefault(u.Type);
                return template == null
                    ? throw new NullReferenceException("Шаблона не существует")
                    : UnitStatsCalculator.CalculateCurrentPower(template, u.Level, u.Health);
            });

        if (attackerPower <= 0 || defenderPower <= 0)
            throw new ArgumentException($"[{nameof(AutoBattleCalculator)}] Сила одной из армий <= 0");

        var compareResult = attackerPower.CompareTo(defenderPower);

        var (attackerCasualties, defenderCasualties) = compareResult switch
        {
            1 => (configuration.WinnerCasualties, configuration.LoserCasualties),
            -1 => (configuration.LoserCasualties, configuration.WinnerCasualties),
            _ => (configuration.DrawCasualties, configuration.DrawCasualties),
        };

        var attackerLogs = CalculateCasualties(liveAttackers, attackerCasualties);
        var defenderLogs = CalculateCasualties(liveDifenders, defenderCasualties);

        return new BattleResult(
            IsAttackerWon: compareResult == 1,
            IsDefenderWon: compareResult == -1,
            attackerLogs,
            defenderLogs
        );
    }

    private List<BattleLog> CalculateCasualties(List<Unit> units, float damagePercent)
    {
        List<BattleLog> logs = [];
        foreach (var unit in units)
        {
            var template = _typeToTemplate[unit.Type];
            var damage = (int)(template.MaxHealth * damagePercent);
            var health = unit.Health - damage;

            logs.Add(new(unit.Id, damage, health, unit.IsAlive));
        }

        return logs;
    }
}