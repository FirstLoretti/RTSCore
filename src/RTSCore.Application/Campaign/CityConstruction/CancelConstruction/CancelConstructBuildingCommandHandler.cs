using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public sealed class CancelConstructBuildingCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<CancelConstructBuildingCommand>
{
    public async Task Handle(CancelConstructBuildingCommand request, CancellationToken ct)
    {
        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct);
        Guard.Against.NotFound(request.CityId, city);

        city.CancelConstruction(request.BuildingId);

        await unitOfWork.SaveChangesAsync(ct);
    }
}