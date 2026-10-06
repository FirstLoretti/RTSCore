using MediatR;

using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.ArmyCreation;

public record CreateArmyCommand(CityId CityId) : IRequest<ArmyId>;