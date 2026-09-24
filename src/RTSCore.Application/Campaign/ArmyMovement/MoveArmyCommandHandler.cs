using System.Numerics;

using MediatR;

using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Campaign.ArmyMovement;

public class MoveArmyCommandHandler(
    IUnitOfWork unitOfWork,
    ArmyConfiguration configuration
)
: IRequestHandler<MoveArmyCommand, MoveArmyCommandResponse>
{
    public async Task<MoveArmyCommandResponse> Handle(MoveArmyCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct);
        Guard.Against.NotFound(army, request.ArmyId);

        var destination = new Vector2(request.X, request.Y);
        var movementCost = ArmyMovementCalculator.CalculateCost(
            army.Coordinates, destination, configuration.MovementUnitDistanceCost
        );

        army.MoveTo(destination, movementCost);

        await unitOfWork.SaveChangesAsync(ct);

        return new MoveArmyCommandResponse(army.Id, army.Coordinates.X, army.Coordinates.Y, army.MovementPoints);
    }
}