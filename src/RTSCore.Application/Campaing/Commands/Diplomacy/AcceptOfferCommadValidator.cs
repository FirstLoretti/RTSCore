using FluentValidation;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class AcceptOfferCommandValidator : AbstractValidator<AcceptOfferCommand>
{
    public AcceptOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}