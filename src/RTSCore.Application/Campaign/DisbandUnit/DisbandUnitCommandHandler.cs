using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;

namespace RTSCore.Application.Campaign.DisbandUnit;

public sealed class DisbandUnitCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<DisbandUnitCommand, Result>
{
    public async Task<Result> Handle(DisbandUnitCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct);
        Guard.Against.NotFound(request.ArmyId, army);

        var result = army.DisbandUnit(request.UnitId);
        if (result.IsFailure) return result;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}