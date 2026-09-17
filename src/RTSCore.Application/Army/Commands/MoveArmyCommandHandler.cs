using System.Numerics;

using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Army.Commands;

public class MoveArmyCommandHandler(
    IUnitOfWork unitOfWork,
    ICampaignMovementService campaingMovementService
)
: IRequestHandler<MoveArmyCommand, MoveArmyCommandResponse>
{
    public async Task<MoveArmyCommandResponse> Handle(MoveArmyCommand request, CancellationToken cancellationToken)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, cancellationToken);
        Guard.Against.NotFound(army, request.ArmyId);

        if (!army.HasGeneral) throw new GameRuleException("Армия не может перемещаться без генерала.");

        var destination = new Vector2(request.X, request.Y);
        var movementCost = campaingMovementService.CalculateMovementCost(army.Coordinates, destination);
        army.MoveTo(destination, movementCost);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MoveArmyCommandResponse(army.Id, army.Coordinates.X, army.Coordinates.Y, army.MovementPoints);
    }
}