using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.AutoBattle;

public record AutoBattleCommand(
    string AttackerArmyId,
    string DefenderArmyId
) : IRequest<BattleResult>;