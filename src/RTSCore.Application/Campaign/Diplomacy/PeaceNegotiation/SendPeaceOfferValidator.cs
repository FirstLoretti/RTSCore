using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.PeaceNegotiation;

public sealed class SendPeaceOfferCommandValidator : AbstractValidator<SendPeaceOfferCommand>
{
    public SendPeaceOfferCommandValidator()
    {
        RuleFor(c => c.CampaignId).NotEmpty();
        RuleFor(c => c.Initiator).IsInEnum();
        RuleFor(c => c.Target).IsInEnum();
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}