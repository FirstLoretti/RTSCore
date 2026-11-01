using MediatR;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Armies.Combat;

namespace RTSCore.Application.Campaign.AutoBattle;

public sealed record AutoBattleCommand(
    ArmyId AttackerId,
    ArmyId DefenderId
) : IRequest<BattleResult>;