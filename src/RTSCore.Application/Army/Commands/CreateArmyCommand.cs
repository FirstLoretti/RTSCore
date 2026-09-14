using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Army.Commands;

public record CreateArmyCommand(CityId CityId) : IRequest<string>;