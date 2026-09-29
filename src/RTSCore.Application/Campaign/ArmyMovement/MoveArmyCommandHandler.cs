using System.Numerics;

using MediatR;

using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Campaign.ArmyMovement;

public class MoveArmyCommandHandler(
    IUnitOfWork unitOfWork,
    IArmyMovementService movementService
) : IRequestHandler<MoveArmyCommand, ArmyMovementResult>
{
    public async Task<ArmyMovementResult> Handle(MoveArmyCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.Id, ct);
        Guard.Against.NotFound(army, request.Id);

        var result = movementService.MoveTo(army, new Vector2(request.X, request.Y));

        await unitOfWork.SaveChangesAsync(ct);

        return result;
    }
}