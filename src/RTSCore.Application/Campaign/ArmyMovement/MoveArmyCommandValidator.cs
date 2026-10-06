using FluentValidation;

namespace RTSCore.Application.Campaign.ArmyMovement;

public class MoveArmyCommandValidator : AbstractValidator<MoveArmyCommand>
{
    public MoveArmyCommandValidator()
    {
        RuleFor(a => a.Id.Value).NotEmpty();
    }
}