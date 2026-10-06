using System.Numerics;

using MediatR;

using RTSCore.Application.Common.Validation;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Services.Armies;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.ValueObjects.Results;

namespace RTSCore.Application.Campaign.ArmyMovement;

public class MoveArmyCommandHandler(
    IUnitOfWork unitOfWork,
    MovementConfiguration movementConfiguration
) : IRequestHandler<MoveArmyCommand, ArmyMovementResult>
{
    public async Task<ArmyMovementResult> Handle(MoveArmyCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.Id, ct);
        Guard.Against.NotFound(army, request.Id);

        var destination = new Vector2(request.X, request.Y);
        var movementCost = ArmyMovementCalculator.CalculateCost(
            army.Coordinates,
            destination,
            movementConfiguration.BaseUnitDistanceCost
        );

        army.MoveTo(destination, movementCost);

        await unitOfWork.SaveChangesAsync(ct);

        return new(army.Id, army.Coordinates.X, army.Coordinates.Y, army.MovementPoints);
    }
}