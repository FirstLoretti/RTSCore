using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Armies.Combat;

namespace RTSCore.Domain.Entities.Armies.Combat;

public sealed class AutoBattleCalculator(
    UnitCatalog catalog,
    AutoBattleConfiguration config,
    UnitStatsCalculator statsCalculator
)
{
    public BattleResult Calculate(IReadOnlyCollection<Unit> attaсkers, IReadOnlyCollection<Unit> defenders)
    {
        var attackerPower = attaсkers
            .Sum(u =>
            {
                var template = catalog.GetUnit(u.Type);
                return statsCalculator.Power(template, u.Level, u.Health);
            });

        var defenderPower = defenders
            .Sum(u =>
            {
                var template = catalog.GetUnit(u.Type);
                return statsCalculator.Power(template, u.Level, u.Health);
            });

        var powerBalance = attackerPower.CompareTo(defenderPower);

        var (damageToAttacker, damageToDefender) = powerBalance switch
        {
            1 => (config.DamageToWinner, config.DamageToLoser),
            -1 => (config.DamageToLoser, config.DamageToWinner),
            _ => (config.DrawDamage, config.DrawDamage),
        };

        var attackerLogs = CreateLogs(attaсkers, damageToAttacker);
        var defenderLogs = CreateLogs(defenders, damageToDefender);

        return new BattleResult(
            IsAttackerWon: powerBalance == 1,
            IsDefenderWon: powerBalance == -1,
            attackerLogs,
            defenderLogs
        );
    }

    private List<UnitBattleLog> CreateLogs(IReadOnlyCollection<Unit> units, float damageTaken)
    {
        List<UnitBattleLog> logs = [];

        foreach (var unit in units)
        {
            var template = catalog.GetUnit(unit.Type);

            var damage = (int)(template.MaxHealth * damageTaken);

            logs.Add(new(unit.Id, damage));
        }

        return logs;
    }
}