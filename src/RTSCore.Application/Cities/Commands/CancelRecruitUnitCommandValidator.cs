using FluentValidation;

namespace RTSCore.Application.Cities.Commands;

public class CancelRecruitUnitCommandValidator : AbstractValidator<CancelRecruitUnitCommand>
{
    public CancelRecruitUnitCommandValidator()
    {
        RuleFor(c => c.UnitId.Value).NotEmpty().Length(3, 30);
    }
}