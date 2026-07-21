using MediatR;

namespace RTSCore.Application.Units.Commands;

public readonly record struct DeleteUnitCommand(string Id) : IRequest;