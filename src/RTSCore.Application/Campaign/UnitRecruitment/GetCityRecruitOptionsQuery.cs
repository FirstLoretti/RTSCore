using MediatR;

using RTSCore.Domain.ValueObjects.Common;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record GetCityRecruitOptionsQuery(
    CityId CityId
) : IRequest<IReadOnlyCollection<ProductionOption>>;