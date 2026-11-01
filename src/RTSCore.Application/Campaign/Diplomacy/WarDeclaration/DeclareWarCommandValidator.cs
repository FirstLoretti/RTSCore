using FluentValidation;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public sealed class DeclareWarCommandValidator : AbstractValidator<DeclareWarCommand>
{
    public DeclareWarCommandValidator()
    {
        RuleFor(c => c.CampaignId).NotEmpty();
        RuleFor(c => c.Initiator).IsInEnum();
        RuleFor(c => c.Target).IsInEnum();
        RuleFor(c => c).Must(c => c.Initiator != c.Target);
    }
}