using FluentValidation;

namespace RTSCore.Application.Campaign.DisbandUnit;

public sealed class DisbandUnitCommandValidator : AbstractValidator<DisbandUnitCommand>
{
    public DisbandUnitCommandValidator()
    {
        RuleFor(u => u.UnitId.Value).NotEmpty();
        RuleFor(u => u.ArmyId.Value).NotEmpty();
    }
}