using MediatR;

using RTSCore.Domain.Configurations;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    UnitConfig unitConfiguration
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