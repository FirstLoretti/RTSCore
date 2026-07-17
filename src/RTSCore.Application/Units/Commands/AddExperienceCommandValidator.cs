using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class AddExperienceCommandValidator : AbstractValidator<AddExperienceCommand>
{
    public AddExperienceCommandValidator()
    {
        RuleFor(c => c.Amount)
            .ExclusiveBetween(0, 5000)
            .WithMessage("Начисляемый опыт должен быть в районе 0-5000");
    }
}