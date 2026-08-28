using FluentValidation;

namespace RTSCore.Application.Cities.Commands;

public class CancelConstructBuildingCommandValidator : AbstractValidator<CancelConstructBuildingCommand>
{
    public CancelConstructBuildingCommandValidator()
    {
        RuleFor(b => b.BuildingId).NotEmpty().Length(3, 30);
    }
}