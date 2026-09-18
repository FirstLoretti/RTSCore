using System.Collections.Frozen;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public class AutoBattleCalculator(IReadOnlyCollection<UnitTemplate> units) : IAutoBattleCalculator
{
    private readonly FrozenDictionary<UnitType, UnitTemplate> _typeToTemplate = units.ToFrozenDictionary(u => u.Type);

    public BattleResult Calculate(IReadOnlyCollection<Unit> attakers, IReadOnlyCollection<Unit> defenders)
    {
        var attackerPower = (int)attakers
            .Where(u => u.IsAlive)
            .Sum(u => u.Damage * (u.Health / 100f));
        var defenderPower = (int)defenders
            .Where(u => u.IsAlive)
            .Sum(u => u.Damage * (u.Health / 100f));

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