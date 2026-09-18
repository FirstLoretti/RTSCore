using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Services;

public record AutoBattleCommand(
    string AttackerArmyId,
    string DefenderArmyId
) : IRequest<BattleResult>;