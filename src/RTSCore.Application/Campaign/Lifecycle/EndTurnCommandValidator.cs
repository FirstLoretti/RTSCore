using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Lifecycle;

public class EndTurnCommandValidator : AbstractValidator<EndTurnCommand>
{
    public EndTurnCommandValidator()
    {
        RuleFor(c => c.Faction).IsInEnum().NotEqual(FactionType.None);
    }
}