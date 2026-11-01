using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.TradeProposal;

public sealed class SendTraidOfferCommandValidator : AbstractValidator<SendTradeOfferCommand>
{
    public SendTraidOfferCommandValidator()
    {
        RuleFor(c => c.CampaignId).NotEmpty();
        RuleFor(c => c.Initiator).IsInEnum();
        RuleFor(c => c.Target).IsInEnum();
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}