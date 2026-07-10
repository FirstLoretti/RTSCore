using MediatR;

namespace RTSCore.Application.Units.Commands;

public readonly record struct AddExperienceCommand(string Id, int Amount) :
    IRequest<(int Level, int Experience)>;