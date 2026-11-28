using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.GetInboxOffers;

public sealed class GetInboxOffersQueryValidator : AbstractValidator<GetInboxOffersQuery>
{
    public GetInboxOffersQueryValidator()
    {
        RuleFor(q => q.CampaignId.Value).NotEmpty();
        RuleFor(q => q.FactionType).IsInEnum();
    }
}