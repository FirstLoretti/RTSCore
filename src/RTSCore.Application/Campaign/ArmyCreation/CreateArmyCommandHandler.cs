using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Application.Common.Settings;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<UnitTemplate> unitTemplates
) : IRequestHandler<CreateArmyCommand, ArmyId>
{
    public async Task<ArmyId> Handle(CreateArmyCommand request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetCityAsync(request.CityId, cancellationToken);
        Guard.Against.NotFound(city, request.CityId);

        var army = city.RaiseArmy(type =>
            unitTemplates.FirstOrDefault(u => u.Type == type)
            ?? throw new NotFoundException($"Шаблона с типом {type} не существует")
        );

        unitOfWork.ArmyRepository.Add(army);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return army.Id;
    }
}