using MediatR;

using RTSCore.Domain.Entities.Armies.Combat;

namespace RTSCore.Application.Campaign.AutoBattle;

public record AutoBattleCommand(
    string AttackerArmyId,
    string DefenderArmyId
) : IRequest<BattleResult>;