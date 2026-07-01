using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Unit(
    UnitId id,
    string displayName,
    FactionId factionHolder,
    int damage,
    int maxHealth,
    int expKillReward
    )
{
    private int _currentHealth = maxHealth;
    private int _maxHealth = maxHealth;
    private int _damage = damage;
    private int _expKillReward = expKillReward;

    public UnitId Id { get; init; } = id;
    public string DisplayName { get; init; } = displayName;
    public FactionId FactionHolder { get; init; } = factionHolder;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; } = 0;
    public bool IsAlive => _currentHealth > 0;

    public Result<int> TakeDamage(int amount)
    {
        if (!IsAlive) return Error.Combat("DeadNoDamage", $"Мёртвый юнит {Id} не может получить урон");

        _currentHealth = int.Max(0, _currentHealth - amount);

        return IsAlive ? 0 : _expKillReward;
    }

    public Result<bool> AddExperience(int amount, IExperienceTable experienceTable)
    {
        if (!IsAlive) return Error.Combat("DeadNoExp", $"Мёртвый юнит {Id} не может получить опыт");

        Experience += int.Max(0, amount);
        var newLevel = experienceTable.GetLevel(Experience);

        while (newLevel > Level)
        {
            Level++;
            _damage = (int)MathF.Ceiling(_damage * 1.1f);
            _maxHealth = (int)MathF.Ceiling(_maxHealth * 1.1f);
            _expKillReward = (int)MathF.Ceiling(_expKillReward * 1.1f);
        }

        return true;
    }
}