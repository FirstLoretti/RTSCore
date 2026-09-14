using FluentValidation;

namespace RTSCore.Application.Army.Commands;

public class CreateArmyCommandValidator : AbstractValidator<CreateArmyCommand>
{
    public CreateArmyCommandValidator()
    {
        RuleFor(c => c.CityId.Value).MaximumLength(256);
    }
}