using System.Collections.Immutable;

namespace RTSCore.Domain.Services;

public static class GameBalance
{
    public static readonly ImmutableArray<int> ExpToNextLevel = [50, 100, 150, 200];

    public static int CalculateStat(float value, float growthRate, int level)
    {
        var raw = value * MathF.Pow(growthRate, level - 1);
        return (int)MathF.Ceiling(raw);
    }
}