using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class RecruitRegularUnitCommandValidator : AbstractValidator<RecruitUnitCommand>
{
    public RecruitRegularUnitCommandValidator()
    {
        RuleFor(c => c.ArmyId).NotEmpty();
        RuleFor(c => c.CityId).NotEmpty();

        RuleFor(c => c.UnitType).IsInEnum();
    }
}