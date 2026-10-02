using MediatR;

using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public class ConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<BuildingTemplate> buildingTemplates
) : IRequestHandler<ConstructBuildingCommand>
{
    public async Task Handle(ConstructBuildingCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселения {request.CityId} нет на карте кампании"
            );

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(city.Faction, ct)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Поселение {city.Id} принадлежит {city.Faction}, " +
                $"но эта фракция не зарегистрирована в текущей игре."
            );

        var template = buildingTemplates.FirstOrDefault(b => b.Type == request.BuildingType)
            ?? throw new NotFoundException(
                $"[{nameof(ConstructBuildingCommandHandler)}] " +
                $"Шаблон здания для типа {request.BuildingType} не содержится"
           );

        city.StartConstruction(template);
        faction.SpendGold(template.Cost);

        await unitOfWork.SaveChangesAsync(ct);
    }
}