using MediatR;

using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record CancelRecruitUnitCommand(UnitId Id) : IRequest;