using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class DisbandUnitCommandValidator : AbstractValidator<DisbandUnitCommand>
{
    public DisbandUnitCommandValidator()
    {
        RuleFor(e => e.Id.Value).NotEmpty().Length(3, 30);
    }
}