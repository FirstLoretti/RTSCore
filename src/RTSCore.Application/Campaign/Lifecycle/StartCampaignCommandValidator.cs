using FluentValidation;

using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.Campaign.Lifecycle;

public class StartCampaignCommandValidator : AbstractValidator<StartCampaignCommand>
{
    public StartCampaignCommandValidator()
    {
        RuleFor(c => c.SelectedFactions).NotEmpty();
        RuleForEach(c => c.SelectedFactions).IsInEnum().NotEqual(FactionType.None);
    }
}