using MediatR;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Buildings.Commands;

public class ConstructBuildingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ConstructBuildingCommand>
{
    public async Task Handle(ConstructBuildingCommand request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetCityAsync(request.CityId, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселения {request.CityId} нет на карте кампании"
                );

        var player = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселение {city.Id} принадлежит {city.OwnerFaction}, " +
                $"но эта фракция не зарегистрирована в текущей игре."
            );

        var template = GameBalance.Buildings.GetTemplate(request.BuildingType);

        player.SpendGold(template.Cost);

        var buildingId = new BuildingId($"building_{city.Id}_{request.BuildingType.ToString().ToLower()}");
        var building = new Building(
            id: buildingId,
            type: request.BuildingType,
            ownerFaction: city.OwnerFaction,
            cityId: city.Id
        );

        unitOfWork.BuildingRepository.Add(building);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}