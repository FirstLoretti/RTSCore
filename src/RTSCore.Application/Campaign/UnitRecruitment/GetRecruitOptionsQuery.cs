using System.Collections.Immutable;

using MediatR;

using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed record GetRecruitOptionsQuery(
    CityId CityId
) : IRequest<ImmutableArray<ProductionOption<UnitType>>>;