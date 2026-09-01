using RTSCore.Application.Cities.Queries;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Queries;

public class GetCityRecruitOptionsQueryHandler(IUnitOfWork unitOfWork, IReadOnlyCollection<UnitTemplate> templates)
    : GetCityCatalogOptionsQueryHandler<GetCityRecruitOptionsQuery, UnitType, UnitTemplate>(unitOfWork, templates)
{
    protected override bool IsVisibleInCity(UnitTemplate template, City city)
    {
        return !template.RequiredBuildingForRecruitment.HasValue
            || city.Buildings.Any(b => b.Type == template.RequiredBuildingForRecruitment && b.IsConstructed);
    }
}