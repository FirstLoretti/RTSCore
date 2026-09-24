using FluentValidation;

namespace RTSCore.Application.Campaign.DisbandUnit;

public class DisbandUnitCommandValidator : AbstractValidator<DisbandUnitCommand>
{
    public DisbandUnitCommandValidator()
    {
        RuleFor(u => u.Id.Value).NotEmpty().MaximumLength(256);
    }
}