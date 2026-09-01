using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities.Rules;

public static class BuildingChainRules
{
    public static bool CanConstruct(
        BuildingTemplate template,
        CityType cityType,
        IEnumerable<Building> constructedBuildings,
        out string? lockReason
    )
    {
        lockReason = null;

        if (!template.AllowedCityTypes.Contains(cityType))
        {
            lockReason =
                $"В поселении типа {cityType} нельзя построить {template.DisplayName}. " +
                $"Требуется поселение типа {string.Join(", ", template.AllowedCityTypes)}";

            return false;
        }

        if (template.RequiredPreviousTier.HasValue)
        {
            var requiredType = template.RequiredPreviousTier.Value;
            var hasPreviousTier = constructedBuildings.Any(b => b.Type == requiredType && b.IsConstructed);

            if (!hasPreviousTier)
            {
                lockReason = $"Нельзя построить {template.DisplayName}. Сначала постройте {requiredType}";
                return false;
            }
        }

        return true;
    }
}