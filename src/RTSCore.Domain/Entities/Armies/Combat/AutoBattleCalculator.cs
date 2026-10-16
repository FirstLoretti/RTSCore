using System.Collections.Immutable;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Configurations.Armies.Combat;

namespace RTSCore.Domain.Entities.Armies.Combat;

public sealed class AutoBattleCalculator(UnitCatalog catalog, AutoBattleConfiguration config)
{
    public BattleResult Calculate(ImmutableArray<Unit> attaсkers, ImmutableArray<Unit> defenders)
    {
        var attackerPower = attaсkers
            .Sum(u =>
            {
                var template = catalog.GetUnit(u.Type);
                return UnitStatsCalculator.CalculatePower(template, u.Level, u.Health);
            });

        var defenderPower = defenders
            .Sum(u =>
            {
                var template = catalog.GetUnit(u.Type);
                return UnitStatsCalculator.CalculatePower(template, u.Level, u.Health);
            });

        var powerBalance = attackerPower.CompareTo(defenderPower);

        var (damageToAttacker, damageToDefender) = powerBalance switch
        {
            1 => (config.DamageToWinner, config.DamageToLoser),
            -1 => (config.DamageToLoser, config.DamageToWinner),
            _ => (config.DrawDamage, config.DrawDamage),
        };

        var attackerLogs = CalculateLogs(attaсkers, damageToAttacker);
        var defenderLogs = CalculateLogs(defenders, damageToDefender);

        return new BattleResult(
            IsAttackerWon: powerBalance == 1,
            IsDefenderWon: powerBalance == -1,
            attackerLogs,
            defenderLogs
        );
    }

    private ImmutableArray<UnitBattleLog> CalculateLogs(ImmutableArray<Unit> units, float damageTaken)
    {
        var logs = ImmutableArray.CreateBuilder<UnitBattleLog>(units.Length);

        foreach (var unit in units)
        {
            var template = catalog.GetUnit(unit.Type);

            var damage = (int)(template.MaxHealth * damageTaken);
            var health = unit.Health - damage;

            logs.Add(new(unit.Id, damage, health));
        }

        return logs.MoveToImmutable();
    }
}