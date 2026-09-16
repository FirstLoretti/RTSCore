using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class RecruitUnitCommandValidator : AbstractValidator<RecruitUnitCommand>
{
    public RecruitUnitCommandValidator()
    {
        RuleFor(c => c.ArmyId).MaximumLength(256);

        RuleFor(c => c.OwnerFaction).IsInEnum().NotEqual(FactionType.None);

        RuleFor(c => c.Type).IsInEnum().NotEqual(UnitType.None);
    }
}