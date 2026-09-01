using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public record CancelRecruitUnitCommand(UnitId UnitId) : IRequest;