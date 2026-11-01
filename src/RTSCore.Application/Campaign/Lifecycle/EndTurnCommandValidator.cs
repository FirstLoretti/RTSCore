using FluentValidation;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class EndTurnCommandValidator : AbstractValidator<EndTurnCommand>
{
    public EndTurnCommandValidator()
    {
        RuleFor(c => c.Faction).IsInEnum();
    }
}