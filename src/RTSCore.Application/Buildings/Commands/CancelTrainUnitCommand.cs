using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Buildings.Commands;

public readonly record struct CancelTrainUnitCommand(
    string BuildingId,
    string UnitId
) : IRequest;