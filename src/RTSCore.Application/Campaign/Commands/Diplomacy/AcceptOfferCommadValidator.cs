using FluentValidation;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public class AcceptOfferCommandValidator : AbstractValidator<AcceptOfferCommand>
{
    public AcceptOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}