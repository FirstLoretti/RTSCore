using MediatR;

using RTSCore.Domain.Interfaces;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public class GetUnitQueryHandler(IUnitRepository repository) :
    IRequestHandler<GetUnitQuery, DomainUnit?>
{
    public async Task<DomainUnit?> Handle(GetUnitQuery request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetUnitAsync(request.Id, cancellationToken);

        return unit;
    }
}