using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public class AcceptOfferCommandValidator : AbstractValidator<AcceptOfferCommand>
{
    public AcceptOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}