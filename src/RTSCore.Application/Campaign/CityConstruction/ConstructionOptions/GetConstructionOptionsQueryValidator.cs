using FluentValidation;

namespace RTSCore.Application.Campaign.CityConstruction.ConstructionOptions;

public sealed class GetConstructionOptionsQueryValidator : AbstractValidator<GetConstructionOptionsQuery>
{
    public GetConstructionOptionsQueryValidator()
    {
        RuleFor(q => q.CityId.Value).NotEmpty();
    }
}