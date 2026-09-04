using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class DeclareWarCommandValidator : AbstractValidator<DeclareWarCommand>
{
    public DeclareWarCommandValidator()
    {
        RuleFor(c => c.Initiator).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c.Target).IsInEnum().NotEqual(FactionType.None);
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}