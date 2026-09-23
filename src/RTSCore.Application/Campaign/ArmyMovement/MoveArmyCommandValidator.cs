using FluentValidation;

namespace RTSCore.Application.Campaign.ArmyMovement;

public class MoveArmyCommandValidator : AbstractValidator<MoveArmyCommand>
{
    public MoveArmyCommandValidator()
    {
        RuleFor(a => a.ArmyId).NotEmpty().MaximumLength(256);
        RuleFor(a => a.X).LessThan(2000);
        RuleFor(a => a.Y).LessThan(2000);
    }
}