using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.CityConstruction.StartConstruction;

public class ConstructBuildingCommandValidator : AbstractValidator<ConstructBuildingCommand>
{
    public ConstructBuildingCommandValidator()
    {
        RuleFor(c => c.CityId.Value).NotEmpty().MaximumLength(256);
        RuleFor(c => c.BuildingType).IsInEnum().NotEqual(BuildingType.None);
    }
}