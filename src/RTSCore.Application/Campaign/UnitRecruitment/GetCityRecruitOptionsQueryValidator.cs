using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public class GetCityRecruitOptionsQueryValidator : AbstractValidator<GetCityRecruitOptionsQuery>
{
    public GetCityRecruitOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty().Length(3, 30);
    }
}