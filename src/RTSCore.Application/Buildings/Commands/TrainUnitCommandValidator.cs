using FluentValidation;

namespace RTSCore.Application.Buildings.Commands;

public class TrainUnitCommandValidator : AbstractValidator<TrainUnitCommand>
{
    public TrainUnitCommandValidator()
    {
        RuleFor(c => c.BuildingId).NotEmpty().Length(3, 30);

        RuleFor(c => c.UnitId).NotEmpty().Length(3, 30);
    }
}