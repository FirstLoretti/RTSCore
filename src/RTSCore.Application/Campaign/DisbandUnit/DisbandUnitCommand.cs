using MediatR;

using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.DisbandUnit;

public record DisbandUnitCommand(UnitId Id) : IRequest;