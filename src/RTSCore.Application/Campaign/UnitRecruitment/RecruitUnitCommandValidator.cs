using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class RecruitUnitCommandValidator : AbstractValidator<RecruitUnitCommand>
{
    public RecruitUnitCommandValidator()
    {
        RuleFor(c => c.ArmyId).NotEmpty();
        RuleFor(c => c.CityId).NotEmpty();

        RuleFor(c => c.UnitType).IsInEnum();
    }
}