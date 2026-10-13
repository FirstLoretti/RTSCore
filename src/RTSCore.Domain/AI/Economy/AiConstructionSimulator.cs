namespace RTSCore.Domain.AI.Economy;

public static class AiConstructionSimulator
{
    public static IReadOnlyCollection<AiConstructionOption> Simulate(
        IReadOnlyCollection<AiConstructionOption> sortedOptions,
        int availableGold
    )
    {
        var buildingsToConstruct = new List<AiConstructionOption>();

        foreach (var option in sortedOptions)
        {
            if (availableGold >= option.Cost)
            {
                buildingsToConstruct.Add(option);
                availableGold -= option.Cost;
            }
        }

        return buildingsToConstruct;
    }
}