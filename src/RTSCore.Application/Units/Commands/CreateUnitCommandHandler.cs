using MediatR;

using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public class CreateUnitCommandHandler(IUnitRepository repository) :
    IRequestHandler<CreateUnitCommand, UnitId>
{
    public Task<UnitId> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = new DomainUnit(
            id: request.Id,
            type: request.Type,
            faction: request.Faction
        );

        repository.Add(unit);
        repository.Save(unit);

        return Task.FromResult(unit.Id);
    }
}