using MediatR;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.ArmyCreation;

public record CreateArmyCommand(CityId CityId) : IRequest<ArmyId>;