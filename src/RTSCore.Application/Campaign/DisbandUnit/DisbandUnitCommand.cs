using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.DisbandUnit;

public sealed record DisbandUnitCommand(ArmyId ArmyId, UnitId UnitId) : IRequest<Result>;