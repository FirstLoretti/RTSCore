using MediatR;

namespace RTSCore.Application.Cities.Commands;

public record CancelConstructBuildingCommand(string BuildingId) : IRequest;