using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Entities.Armies.Combat;
using RTSCore.Domain.Services;

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