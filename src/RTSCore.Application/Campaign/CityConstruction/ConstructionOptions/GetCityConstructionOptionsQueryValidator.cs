using FluentValidation;

using RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

namespace RTSCore.Application.Campaign.CityConstruction;

public class GetCityConstructionOptionsQueryValidator : AbstractValidator<GetCityConstructionOptionsQuery>
{
    public GetCityConstructionOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty().MaximumLength(256);
    }
}