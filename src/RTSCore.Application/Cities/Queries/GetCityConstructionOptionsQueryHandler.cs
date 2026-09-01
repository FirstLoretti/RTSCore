using RTSCore.Application.Cities.Queries;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Entities.Rules;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries;

public class GetCityConstructionOptionsQueryHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<BuildingTemplate> buildingTemplates
) : GetCityCatalogOptionsQueryHandler<GetCityConstructionOptionsQuery, BuildingType, BuildingTemplate>(unitOfWork, buildingTemplates)
{
    protected override bool IsVisibleInCity(BuildingTemplate template, City city)
    {
        var isAlreadyBuilded = city.Buildings.Any(b => b.Type == template.Type && b.IsConstructed);

        return !isAlreadyBuilded && BuildingRules.CanConstruct(template, city.Type, city.Buildings, out _);
    }
}