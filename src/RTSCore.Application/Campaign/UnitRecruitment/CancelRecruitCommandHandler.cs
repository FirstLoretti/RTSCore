using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class CancelRecruitCommandHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<CancelRecruitCommand, Result>
{
    public async Task<Result> Handle(CancelRecruitCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct);
        Guard.Against.NotFound(request.ArmyId, army);

        var result = army.CancelRecruitUnit(request.UnitId);
        if (result.IsFailure) return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}