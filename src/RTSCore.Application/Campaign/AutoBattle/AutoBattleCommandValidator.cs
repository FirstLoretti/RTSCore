using FluentValidation;

namespace RTSCore.Application.Campaign.AutoBattle;

public sealed class AutoBattleCommandValidator : AbstractValidator<AutoBattleCommand>
{
    public AutoBattleCommandValidator()
    {
        RuleFor(c => c.AttackerId.Value).NotEmpty();
        RuleFor(c => c.DefenderId.Value).NotEmpty();
        RuleFor(c => c.AttackerId).NotEqual(c => c.DefenderId);
    }
}