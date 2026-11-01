using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Entities.Armies.Combat;

namespace RTSCore.Application.Campaign.AutoBattle;

public sealed class AutoBattleCommandHandler(
    IUnitOfWork unitOfWork,
    AutoBattleCalculator calculator
) : IRequestHandler<AutoBattleCommand, BattleResult>
{
    public async Task<BattleResult> Handle(AutoBattleCommand request, CancellationToken ct)
    {
        var attacker = await unitOfWork.ArmyRepository.GetAsync(request.AttackerId, ct);
        Guard.Against.NotFound(request.AttackerId, attacker);
        var defender = await unitOfWork.ArmyRepository.GetAsync(request.DefenderId, ct);
        Guard.Against.NotFound(request.DefenderId, defender);

        var battleResult = calculator.Calculate(attacker.GetReadyCombatants(), defender.GetReadyCombatants());

        attacker.TakeCasualties(battleResult.AttackerBattleLogs);
        defender.TakeCasualties(battleResult.DefenderBattleLogs);

        await unitOfWork.SaveChangesAsync(ct);

        return battleResult;
    }
}