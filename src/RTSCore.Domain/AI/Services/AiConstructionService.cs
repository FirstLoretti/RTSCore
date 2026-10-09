using RTSCore.Domain.AI.ValueObjects;

namespace RTSCore.Domain.AI.Services;

public class AiConstructionService
{
    public IReadOnlyCollection<AiConstructionOption> SimulateConstruction(
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