using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Application.Common.Settings;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    IArmyCreationService armyCreationService
) : IRequestHandler<CreateArmyCommand, ArmyId>
{
    public async Task<ArmyId> Handle(CreateArmyCommand request, CancellationToken cancellationToken)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, cancellationToken);
        Guard.Against.NotFound(city, request.CityId);

        var army = armyCreationService.CreateArmy(city);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return army.Id;
    }
}