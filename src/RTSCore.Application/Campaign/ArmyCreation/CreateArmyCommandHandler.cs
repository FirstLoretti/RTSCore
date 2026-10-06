using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.ValueObjects.Identifiers;
using RTSCore.Domain.Entities.Common;
using RTSCore.Application.Common.Validation;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    UnitConfiguration unitConfiguration
) : IRequestHandler<CreateArmyCommand, ArmyId>
{
    public async Task<ArmyId> Handle(CreateArmyCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct);
        Guard.Against.NotFound(city, request.CityId);

        var army = city.CreateArmy(unitConfiguration.Templates);

        await unitOfWork.SaveChangesAsync(ct);

        return army.Id;
    }
}