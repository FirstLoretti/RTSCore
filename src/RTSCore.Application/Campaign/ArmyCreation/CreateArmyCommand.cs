using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.ArmyCreation;

public record CreateArmyCommand(CityId CityId) : IRequest<ArmyId>;