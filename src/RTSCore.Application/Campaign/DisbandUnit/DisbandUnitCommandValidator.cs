using FluentValidation;

namespace RTSCore.Application.Campaign.DisbandUnit;

public class DisbandUnitCommandValidator : AbstractValidator<DisbandUnitCommand>
{
    public DisbandUnitCommandValidator()
    {
        RuleFor(e => e.Id.Value).NotEmpty().MaximumLength(256);
    }
}