using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class SendPeaceOfferCommandValidator : AbstractValidator<SendPeaceOfferCommand>
{
    public SendPeaceOfferCommandValidator()
    {
        RuleFor(c => c.Initiator).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c.Target).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}