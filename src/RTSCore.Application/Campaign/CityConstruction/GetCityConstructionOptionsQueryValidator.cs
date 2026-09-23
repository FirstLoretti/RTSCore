using FluentValidation;

namespace RTSCore.Application.Campaign.CityConstruction;

public class GetCityConstructionOptionsQueryValidator : AbstractValidator<GetCityConstructionOptionsQuery>
{
    public GetCityConstructionOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty().Length(3, 30);
    }
}