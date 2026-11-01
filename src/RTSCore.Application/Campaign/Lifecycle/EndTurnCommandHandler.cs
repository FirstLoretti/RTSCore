using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Cities;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class EndTurnCommandHandler(
    IUnitOfWork unitOfWork,
    BuildingCatalog buildingCatalog,
    CityConfiguration cityConfiguration
) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken ct)
    {
        var cities = await unitOfWork.CityRepository.GetAsync(request.Faction, ct);
        var armies = await unitOfWork.ArmyRepository.GetAsync(request.Faction, ct);

        foreach (var city in cities)
        {
            city.TurnEnd(buildingCatalog, cityConfiguration);
        }
        foreach (var army in armies)
        {
            army.TurnEnd();
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}