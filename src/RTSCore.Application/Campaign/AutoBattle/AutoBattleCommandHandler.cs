using MediatR;

using RTSCore.Application.Common.Validation;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Services;
using RTSCore.Domain.Services.Combat;
using RTSCore.Domain.ValueObjects.Results;

namespace RTSCore.Application.Campaign.AutoBattle;

public class AutoBattleCommandHandler(
    IUnitOfWork unitOfWork,
    IAutoBattleService autoBattleService
) : IRequestHandler<AutoBattleCommand, BattleResult>
{
    public async Task<BattleResult> Handle(AutoBattleCommand request, CancellationToken ct)
    {
        var attacker = await unitOfWork.ArmyRepository.GetAsync(request.AttackerArmyId, ct);
        Guard.Against.NotFound(attacker, request.AttackerArmyId);
        var defender = await unitOfWork.ArmyRepository.GetAsync(request.DefenderArmyId, ct);
        Guard.Against.NotFound(defender, request.DefenderArmyId);

        var battleResult = autoBattleService.StartAutoBattle(attacker, defender);

        await unitOfWork.SaveChangesAsync(ct);

        return battleResult;
    }
}