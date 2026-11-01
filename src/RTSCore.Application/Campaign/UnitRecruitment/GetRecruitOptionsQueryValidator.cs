using FluentValidation;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class GetRecruitOptionsQueryValidator : AbstractValidator<GetRecruitOptionsQuery>
{
    public GetRecruitOptionsQueryValidator()
    {
        RuleFor(q => q.CityId).NotEmpty();
    }
}