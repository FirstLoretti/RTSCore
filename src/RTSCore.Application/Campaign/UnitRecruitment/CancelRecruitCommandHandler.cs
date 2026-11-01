using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class CancelRecruitCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelRecruitCommand>
{
    public async Task Handle(CancelRecruitCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct);
        Guard.Against.NotFound(request.ArmyId, army);

        army.CancelRecruitUnit(request.UnitId);

        await unitOfWork.SaveChangesAsync(ct);
    }
}