using MediatR;

using RTSCore.Domain.ValueObjects.Results;

namespace RTSCore.Application.Campaign.AutoBattle;

public record AutoBattleCommand(
    string AttackerArmyId,
    string DefenderArmyId
) : IRequest<BattleResult>;