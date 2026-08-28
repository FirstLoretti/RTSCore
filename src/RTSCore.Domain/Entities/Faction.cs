using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class Faction
{
    public PlayerType PlayerType { get; init; }
    public FactionType Type { get; init; }
    public int Gold { get; private set; }

    public Faction(FactionType type, int gold, PlayerType playerType)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(gold);

        Type = type;
        Gold = gold;
        PlayerType = playerType;
    }

    protected Faction() { }

    public void SpendGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (Gold - amount < 0)
            throw new GameRuleException("В казне недостаточно средств");

        Gold -= amount;
    }

    public void EarnGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        Gold += amount;
    }
}