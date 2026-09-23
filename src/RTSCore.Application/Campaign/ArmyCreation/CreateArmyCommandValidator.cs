using FluentValidation;

namespace RTSCore.Application.Campaign.ArmyCreation;

public class CreateArmyCommandValidator : AbstractValidator<CreateArmyCommand>
{
    public CreateArmyCommandValidator()
    {
        RuleFor(c => c.CityId.Value).MaximumLength(256);
    }
}