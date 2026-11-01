using FluentValidation;

namespace RTSCore.Application.Campaign.CityConstruction.CancelConstruction;

public sealed class CancelConstructBuildingCommandValidator : AbstractValidator<CancelConstructBuildingCommand>
{
    public CancelConstructBuildingCommandValidator()
    {
        RuleFor(c => c.BuildingId.Value).NotEmpty();
        RuleFor(c => c.CityId.Value).NotEmpty();
    }
}