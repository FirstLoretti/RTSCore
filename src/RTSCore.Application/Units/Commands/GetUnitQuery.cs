using MediatR;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public readonly record struct GetUnitQuery(string Id) : IRequest<DomainUnit>;