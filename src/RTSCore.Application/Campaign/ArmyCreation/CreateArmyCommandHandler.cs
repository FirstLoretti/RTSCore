using MediatR;
using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

using Unit = RTSCore.Domain.Entities.Unit;
using Army = RTSCore.Domain.Entities.Army;
using RTSCore.Application.Common.Settings;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates
) : IRequestHandler<CreateArmyCommand, string>
{
    public async Task<string> Handle(CreateArmyCommand request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetCityAsync(request.CityId, cancellationToken);
        Guard.Against.NotFound(city, request.CityId);

        var governorTemplate = unitTemplates.FirstOrDefault(u => u.Type == city.Governor)
            ?? throw new NotFoundException($"Шаблона с типом {city.Governor} не существует");

        var general = Unit.Create(city.OwnerFaction, governorTemplate);
        var army = Army.Create(
            city.OwnerFaction, city.Coordinates, GameBalance.Army.MaxMovementPoints, GameBalance.Army.MaxSize, general
        );

        unitOfWork.UnitRepository.Add(general);
        unitOfWork.ArmyRepository.Add(army);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return army.Id;
    }
}