using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Domain.Entities;

public class Faction
{
    public PlayerType Player { get; init; }
    public FactionType Type { get; init; }
    public int Gold { get; private set; }
    public bool IsEliminated { get; private set; }

    private Faction(FactionType type, PlayerType player, int gold)
    {
        Type = type;
        Player = player;
        Gold = gold;
    }

    public static Faction Create(
        FactionType type,
        PlayerType player,
        FactionConfiguration configuration
    ) => new(type, player, configuration.InitialGold);

    public static Faction CreateWithEmptyTreasury(
        FactionType type,
        PlayerType player
    ) => new(type, player, gold: 0);

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

    public void RefundGold(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Gold += amount;
    }
}