using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public sealed class ConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork,
    BuildingCatalog catalog
) : IRequestHandler<ConstructBuildingCommand, Result<BuildingId>>
{
    public async Task<Result<BuildingId>> Handle(ConstructBuildingCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct);
        Guard.Against.NotFound(request.CityId, city);

        var faction = await unitOfWork.FactionRepository.GetAsync(city.Faction, ct);
        Guard.Against.NotFound(city.Faction, faction);

        var template = catalog.GetBuilding(request.BuildingType);

        var factionResult = faction.SpendGold(template.Cost);
        if (factionResult.IsFailure) return factionResult.Error;

        var cityResult = city.StartConstruction(template);
        if (cityResult.IsFailure)
        {
            faction.RefundGold(template.Cost);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return cityResult;
    }
}