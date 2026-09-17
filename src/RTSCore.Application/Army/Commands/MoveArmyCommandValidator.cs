using FluentValidation;

namespace RTSCore.Application.Army.Commands;

public class MoveArmyCommandValidator : AbstractValidator<MoveArmyCommand>
{
    public MoveArmyCommandValidator()
    {
        RuleFor(a => a.ArmyId).NotEmpty().MaximumLength(256);
        RuleFor(a => a.Destination.X).LessThan(2000);
        RuleFor(a => a.Destination.Y).LessThan(2000);
    }
}