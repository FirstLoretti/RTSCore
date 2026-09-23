using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}