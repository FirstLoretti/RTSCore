using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public sealed class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}