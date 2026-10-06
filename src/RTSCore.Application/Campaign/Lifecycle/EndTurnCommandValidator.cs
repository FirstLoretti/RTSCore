using FluentValidation;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Lifecycle;

public class EndTurnCommandValidator : AbstractValidator<EndTurnCommand>
{
    public EndTurnCommandValidator()
    {
        RuleFor(c => c.FactionType).IsInEnum().NotEqual(FactionType.None);
    }
}