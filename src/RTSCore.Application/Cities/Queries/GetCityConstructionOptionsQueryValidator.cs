using FluentValidation;

namespace RTSCore.Application.Cities.Queries;

public class GetCityConstructionOptionsQueryValidator : AbstractValidator<GetCityConstructionOptionsQuery>
{
    public GetCityConstructionOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty().Length(3, 30);
    }
}