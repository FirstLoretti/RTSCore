using FluentValidation;

namespace RTSCore.Application.Campaign.AutoBattle;

public class AutoBattleCommandValidator : AbstractValidator<AutoBattleCommand>
{
    public AutoBattleCommandValidator()
    {
        RuleFor(c => c.AttackerArmyId).NotEmpty().MaximumLength(256);
        RuleFor(c => c.DefenderArmyId).NotEmpty().MaximumLength(256);
    }
}