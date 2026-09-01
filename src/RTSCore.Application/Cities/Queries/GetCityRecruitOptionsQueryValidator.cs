using FluentValidation;

namespace RTSCore.Application.Units.Queries;

public class GetCityRecruitOptionsQueryValidator : AbstractValidator<GetCityRecruitOptionsQuery>
{
    public GetCityRecruitOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty().Length(3, 30);
    }
}