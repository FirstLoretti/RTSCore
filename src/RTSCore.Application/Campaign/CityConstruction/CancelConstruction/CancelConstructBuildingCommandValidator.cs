using FluentValidation;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public class CancelConstructBuildingCommandValidator : AbstractValidator<CancelConstructBuildingCommand>
{
    public CancelConstructBuildingCommandValidator()
    {
        RuleFor(b => b.BuildingId.Value).NotEmpty().MaximumLength(256);
    }
}