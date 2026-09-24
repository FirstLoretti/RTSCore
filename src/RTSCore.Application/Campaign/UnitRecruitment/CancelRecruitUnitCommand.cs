using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record CancelRecruitUnitCommand(UnitId Id) : IRequest;