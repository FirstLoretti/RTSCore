using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class RecruitUnitCommandValidator : AbstractValidator<RecruitUnitCommand>
{
    public RecruitUnitCommandValidator()
    {
        RuleFor(c => c.ArmyId).MaximumLength(256);

        RuleFor(c => c.Faction).IsInEnum().NotEqual(FactionType.None);

        RuleFor(c => c.Type).IsInEnum().NotEqual(UnitType.None);
    }
}