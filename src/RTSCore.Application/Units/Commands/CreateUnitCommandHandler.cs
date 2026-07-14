using MediatR;

using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public class CreateUnitCommandHandler(
    IUnitRepository repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateUnitCommand, UnitId>
{
    public async Task<UnitId> Handle(
        CreateUnitCommand request,
        CancellationToken cancellationToken
    )
    {
        var unit = new DomainUnit(
            id: request.Id,
            type: request.Type,
            faction: request.Faction
        );

        repository.Add(unit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return unit.Id;
    }
}