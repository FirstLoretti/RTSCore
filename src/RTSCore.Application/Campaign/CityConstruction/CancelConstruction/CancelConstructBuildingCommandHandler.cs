using MediatR;

using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public class CancelConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<CancelConstructBuildingCommand>
{
    public async Task Handle(CancelConstructBuildingCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct);
        Guard.Against.NotFound(city, request.CityId);

        city.CancelConstruction(request.BuildingId);

        await unitOfWork.SaveChangesAsync(ct);
    }
}