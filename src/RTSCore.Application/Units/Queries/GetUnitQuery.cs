using MediatR;

using RTSCore.Domain.ValueObjects;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Queries;

public readonly record struct GetUnitQuery(UnitId Id) : IRequest<DomainUnit>;