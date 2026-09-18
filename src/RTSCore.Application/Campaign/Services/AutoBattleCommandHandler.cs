using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Services;

public class AutoBattleCommandHandler(
    IUnitOfWork unitOfWork,
    IAutoBattleCalculator calculator
) : IRequestHandler<AutoBattleCommand, BattleResult>
{
    public async Task<BattleResult> Handle(AutoBattleCommand request, CancellationToken ct)
    {
        var attacker = await unitOfWork.ArmyRepository.GetAsync(request.AttackerArmyId, ct);
        Guard.Against.NotFound(attacker, request.AttackerArmyId);
        var defender = await unitOfWork.ArmyRepository.GetAsync(request.DefenderArmyId, ct);
        Guard.Against.NotFound(defender, request.DefenderArmyId);

        var response = calculator.Calculate(attacker.Units, defender.Units);

        var deadAttackerIds = response.AttackerUnitsLogs
            .Where(u => !u.IsAlive)
            .Select(l => l.UnitId)
            .ToHashSet();
        var deadDefenderIds = response.DefenderUnitsLogs
            .Where(u => !u.IsAlive)
            .Select(l => l.UnitId)
            .ToHashSet();

        var deadAttackerUnits = attacker.Units.Where(u => deadAttackerIds.Contains(u.Id)).ToList();
        var deadDefenderUnits = defender.Units.Where(u => deadDefenderIds.Contains(u.Id)).ToList();

        foreach (var unit in deadAttackerUnits.Concat(deadDefenderUnits))
        {
            unitOfWork.UnitRepository.Delete(unit);
        }

        attacker.PurgeDeadUnits();
        defender.PurgeDeadUnits();

        await unitOfWork.SaveChangesAsync(ct);

        return new BattleResult(
            response.IsAttackerWon, response.AttackerUnitsLogs, response.DefenderUnitsLogs);
    }
}