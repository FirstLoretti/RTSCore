using RTSCore.Application.Campaign.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class GetCityRecruitOptionsQueryHandler(IUnitOfWork unitOfWork, IReadOnlyCollection<UnitTemplate> templates)
    : GetCityCatalogOptionsQueryHandler<GetCityRecruitOptionsQuery, UnitType, UnitTemplate>(unitOfWork, templates)
{
    protected override bool IsVisibleInCity(UnitTemplate template, City city)
    {
        return !template.RequiredBuilding.HasValue
            || city.Buildings.Any(b => b.Type == template.RequiredBuilding && b.IsConstructed);
    }
}