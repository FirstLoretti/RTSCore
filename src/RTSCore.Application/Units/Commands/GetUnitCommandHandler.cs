using MediatR;

using RTSCore.Domain.Interfaces;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public class GetUnitCommandHandler(IUnitRepository repository) :
    IRequestHandler<GetUnitCommand, DomainUnit?>
{
    public Task<DomainUnit?> Handle(GetUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = repository.GetUnit(request.Id);

        return Task.FromResult(unit);
    }
}