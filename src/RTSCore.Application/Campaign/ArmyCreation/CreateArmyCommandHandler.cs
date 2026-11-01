using MediatR;

using Ardalis.GuardClauses;

using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Application.Repositories;


namespace RTSCore.Application.Campaign.ArmyCreation;

public sealed class CreateArmyCommandHandler(
    IUnitOfWork unitOfWork,
    UnitCatalog unitCatalog
) : IRequestHandler<CreateArmyCommand, ArmyId>
{
    public async Task<ArmyId> Handle(CreateArmyCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct, trackChanges: false);
        Guard.Against.NotFound(request.CityId, city);

        var army = city.CreateArmy(unitCatalog.GetTemplate(request.GeneralType));

        unitOfWork.ArmyRepository.Add(army);
        await unitOfWork.SaveChangesAsync(ct);

        return army.Id;
    }
}