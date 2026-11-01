using FluentValidation;

namespace RTSCore.Application.Campaign.ArmyCreation;

public sealed class CreateArmyCommandValidator : AbstractValidator<CreateArmyCommand>
{
    public CreateArmyCommandValidator()
    {
        RuleFor(c => c.CityId.Value).NotEmpty();
        RuleFor(c => c.GeneralType).IsInEnum();
    }
}