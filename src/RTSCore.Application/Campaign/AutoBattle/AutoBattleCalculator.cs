using System.Collections.Frozen;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.AutoBattle;

public class AutoBattleCalculator(IReadOnlyCollection<UnitTemplate> units) : IAutoBattleCalculator
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _typeToTemplate = units.ToFrozenDictionary(u => u.Type);

    public BattleResult Calculate(IReadOnlyCollection<Unit> attakers, IReadOnlyCollection<Unit> defenders)
    {
        var attackerPower = attakers
            .Where(u => u.IsAlive)
            .Sum(u =>
            {
                var template = _typeToTemplate.GetValueOrDefault(u.Type);
                return template == null
                    ? throw new NullReferenceException("Шаблона не существует")
                    : UnitStatsCalculator.CalculateCurrentPower(template, u.Level, u.Health);
            });
        var defenderPower = defenders
            .Where(u => u.IsAlive)
            .Sum(u =>
            {
                var template = _typeToTemplate.GetValueOrDefault(u.Type);
                return template == null
                    ? throw new NullReferenceException("Шаблона не существует")
                    : UnitStatsCalculator.CalculateCurrentPower(template, u.Level, u.Health);
            });

        if (attackerPower <= 0 || defenderPower <= 0)
            throw new ArgumentException($"[{nameof(AutoBattleCalculator)}] Сила одной из армий <= 0");

        bool isAttackerWinner = attackerPower > defenderPower;

        List<UnitBattleLog> attackerLog = [];
        List<UnitBattleLog> defenderLog = [];

        if (isAttackerWinner)
        {
            CalculateCasualties(attakers, 0.3f, attackerLog);
            CalculateCasualties(defenders, 1f, defenderLog);
        }
        else
        {
            CalculateCasualties(attakers, 0.3f, attackerLog);
            CalculateCasualties(defenders, 1f, defenderLog);
        }

        return new BattleResult(isAttackerWinner, attackerLog, defenderLog);
    }

    private void CalculateCasualties(IReadOnlyCollection<Unit> units, float damagePercent, List<UnitBattleLog> logs)
    {
        foreach (var unit in units.Where(u => u.IsAlive))
        {
            if (!_typeToTemplate.TryGetValue(unit.Type, out var template))
                throw new NotFoundException($"[{nameof(AutoBattleCalculator)}] Шаблон юнита {unit.Type} не найден");

            var damage = (int)(template.MaxHealth * damagePercent);
            unit.TakeDamage(damage);

            logs.Add(new(unit.Id, damage, unit.IsAlive));
        }
    }
}