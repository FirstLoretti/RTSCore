using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class SendTraidOfferCommandValidator : AbstractValidator<SendTradeOfferCommand>
{
    public SendTraidOfferCommandValidator()
    {
        RuleFor(c => c.Initiator).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c.Target).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}