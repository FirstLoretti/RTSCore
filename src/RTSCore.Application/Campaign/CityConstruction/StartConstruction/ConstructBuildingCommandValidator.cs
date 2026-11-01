using FluentValidation;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public sealed class ConstructBuildingCommandValidator : AbstractValidator<ConstructBuildingCommand>
{
    public ConstructBuildingCommandValidator()
    {
        RuleFor(c => c.CityId.Value).NotEmpty();
        RuleFor(c => c.BuildingType).IsInEnum();
    }
}