using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Commands;

public record DisbandUnitCommand(UnitId Id) : IRequest;