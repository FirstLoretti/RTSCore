using MediatR;

namespace RTSCore.Application.Campaign.CityConstruction;

public record CancelConstructBuildingCommand(string BuildingId) : IRequest;