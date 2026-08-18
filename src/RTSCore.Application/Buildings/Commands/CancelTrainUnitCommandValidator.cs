using FluentValidation;

namespace RTSCore.Application.Buildings.Commands;

public class CancelTrainUnitCommandValidator : AbstractValidator<CancelTrainUnitCommand>
{
    public CancelTrainUnitCommandValidator()
    {
        RuleFor(c => c.BuildingId).NotEmpty().Length(3, 30);
        RuleFor(c => c.BuildingId).NotEmpty().Length(3, 30);
    }
}