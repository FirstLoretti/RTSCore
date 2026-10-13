using FluentValidation;

using RTSCore.Domain.Configurations;

namespace RTSCore.Application.Campaign.Lifecycle;

public class StartCampaignCommandValidator : AbstractValidator<StartCampaignCommand>
{
    public StartCampaignCommandValidator(GameConfiguations configurations)
    {
        RuleFor(c => c.PlayersFactions)
            .NotNull()

            .Must(f => f.Length is > 0 and < 3)
            .WithMessage("В кампании должно быть от 1 до 2х игроков")

            .Must(f => f.Distinct().Count() == f.Length)
            .WithMessage("Фракции игроков должны отличаться")

            .Must(factions => factions
                .All(f => configurations.Factions.Templates.Any(t => t.Type == f)))
            .WithMessage("Выбранная игроком фракция не содержится в конфигурации игры");
    }
}