using System.Numerics;

using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.ArmyMovement;

public sealed class MoveArmyCommandHandler(
    IUnitOfWork unitOfWork,
    MovementConfiguration movementConfig
) : IRequestHandler<MoveArmyCommand>
{
    public async Task Handle(MoveArmyCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.Id, ct);
        Guard.Against.NotFound(request.Id, army);

        var destination = new Vector2(request.X, request.Y);
        var movementCost = ArmyMovementCalculator.CalculateCost(
            army.Coordinates,
            destination,
            movementConfig.BaseUnitDistanceCost
        );

        army.MoveTo(destination, movementCost);

        await unitOfWork.SaveChangesAsync(ct);
    }
}