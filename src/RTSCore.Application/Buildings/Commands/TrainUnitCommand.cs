using MediatR;

namespace RTSCore.Application.Buildings.Commands;

public readonly record struct TrainUnitCommand(
    string BuildingId,
    string UnitId
) : IRequest;