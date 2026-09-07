using FluentValidation;

namespace RTSCore.Application.Campaign.Commands.Diplomacy;

public class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}