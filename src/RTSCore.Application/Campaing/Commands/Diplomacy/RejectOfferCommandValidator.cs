using FluentValidation;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(c => c.OfferId).NotEmpty();
    }
}