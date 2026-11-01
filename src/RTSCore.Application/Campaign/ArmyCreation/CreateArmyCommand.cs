using MediatR;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.ArmyCreation;

public sealed record CreateArmyCommand(CityId CityId, UnitType GeneralType) : IRequest<ArmyId>;