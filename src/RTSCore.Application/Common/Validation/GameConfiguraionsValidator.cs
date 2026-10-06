using FluentValidation;

using RTSCore.Domain.ValueObjects.Configurations;

namespace RTSCore.Application.Common.Validation;

public class GameConfigurationsValidator : AbstractValidator<GameConfigurations>
{
    public GameConfigurationsValidator()
    {
        RuleFor(c => c.Cities).NotNull();
        RuleFor(c => c.Cities.Templates).NotEmpty();

        RuleFor(c => c.Diplomacy).NotNull();

        RuleFor(c => c.AutoBattle).NotNull();

        RuleFor(c => c.Buildings).NotNull();
        RuleFor(c => c.Buildings).NotEmpty();

        RuleFor(c => c.Factions).NotNull();
        RuleFor(c => c.Factions).NotEmpty();

        RuleFor(c => c.Movement).NotNull();

        RuleFor(c => c.Units).NotNull();
        RuleFor(c => c.Units.Templates).NotEmpty();
    }
}