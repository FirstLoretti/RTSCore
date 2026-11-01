using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class CancelRecruitCommandValidator : AbstractValidator<CancelRecruitCommand>
{
    public CancelRecruitCommandValidator()
    {
        RuleFor(c => c.UnitId).NotEmpty();
        RuleFor(c => c.ArmyId).NotEmpty();
    }
}