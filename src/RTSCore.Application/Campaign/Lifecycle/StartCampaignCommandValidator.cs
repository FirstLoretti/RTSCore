using FluentValidation;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class StartCampaignCommandValidator : AbstractValidator<StartCampaignCommand>
{
    public StartCampaignCommandValidator()
    {
        RuleFor(c => c.PlayersFactions).NotEmpty();
        RuleForEach(c => c.PlayersFactions).IsInEnum();
    }
}