using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed record RecruitUnitCommand(
    ArmyId ArmyId,
    CityId CityId,
    UnitType UnitType
) : IRequest<Result<UnitId>>;
