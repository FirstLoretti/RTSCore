using MediatR;

using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Application.Campaign.DisbandUnit;

public record DisbandUnitCommand(UnitId Id) : IRequest;