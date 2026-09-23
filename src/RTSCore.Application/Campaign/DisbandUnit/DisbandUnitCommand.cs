using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.DisbandUnit;

public record DisbandUnitCommand(UnitId Id) : IRequest;