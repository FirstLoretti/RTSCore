using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Units.Commands;

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Length(3, 30);

        RuleFor(c => c.Faction).IsInEnum().NotEqual(FactionType.None);

        RuleFor(c => c.Type).IsInEnum().NotEqual(UnitType.None);
    }
}