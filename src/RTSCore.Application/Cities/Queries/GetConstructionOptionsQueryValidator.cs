using FluentValidation;

namespace RTSCore.Application.Cities.Queries;

public class GetConstructionOptionsQueryValidator : AbstractValidator<GetConstructionOptionsQuery>
{
    public GetConstructionOptionsQueryValidator()
    {
        RuleFor(q => q.CityId).NotEmpty().Length(3, 30);
    }
}