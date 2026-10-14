using MediatR;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public record CancelRecruitUnitCommand(UnitId Id) : IRequest;