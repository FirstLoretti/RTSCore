using FluentValidation;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands;

public class StartCampaignCommandValidator : AbstractValidator<StartCampaignCommand>
{
    public StartCampaignCommandValidator()
    {
        RuleFor(c => c.SelectedFactions).NotNull().NotEmpty();
        RuleForEach(c => c.SelectedFactions).IsInEnum().NotEqual(FactionType.None);
    }
}