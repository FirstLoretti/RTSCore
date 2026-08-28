using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Commands;

public class ConstructBuildingCommandValidator : AbstractValidator<ConstructBuildingCommand>
{
    public ConstructBuildingCommandValidator()
    {
        RuleFor(c => c.CityId).NotEmpty().Length(3, 20);
        RuleFor(c => c.BuildingType).IsInEnum().NotEqual(BuildingType.None);
    }
}