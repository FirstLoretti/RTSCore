using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class RecruitRegularUnitCommandValidator : AbstractValidator<RecruitRegularUnitCommand>
{
    public RecruitRegularUnitCommandValidator()
    {
        RuleFor(c => c.ArmyId.Value).MaximumLength(256);

        RuleFor(c => c.Faction).IsInEnum().NotEqual(FactionType.None);

        RuleFor(c => c.Type).IsInEnum().NotEqual(UnitType.None);
    }
}