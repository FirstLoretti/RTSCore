using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Interfaces;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public class GetUnitQueryHandler(IUnitRepository repository) :
    IRequestHandler<GetUnitQuery, DomainUnit>
{
    public async Task<DomainUnit> Handle(GetUnitQuery request, CancellationToken cancellationToken)
    {
        var unit = await repository.GetUnitAsync(request.Id, cancellationToken);

        return unit ?? throw new NotFoundException($"Юнит с Id: {request.Id} не найден в базе данных");
    }
}