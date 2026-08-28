using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class AddExperienceCommandValidator : AbstractValidator<AddExperienceCommand>
{
    public AddExperienceCommandValidator()
    {
        RuleFor(c => c.Amount).ExclusiveBetween(0, 5000);
    }
}