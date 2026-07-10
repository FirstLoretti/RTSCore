using MediatR;

using DomainUnit = RTSCore.Domain.Entities.Unit;

namespace RTSCore.Application.Units.Commands;

public readonly record struct GetUnitCommand(string Id) : IRequest<DomainUnit?>;