using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class CancelRecruitUnitCommandValidator : AbstractValidator<CancelRecruitUnitCommand>
{
    public CancelRecruitUnitCommandValidator()
    {
        RuleFor(c => c.Id.Value).NotEmpty().MaximumLength(256);
    }
}