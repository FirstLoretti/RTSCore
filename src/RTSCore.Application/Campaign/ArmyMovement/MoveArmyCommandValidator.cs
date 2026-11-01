using FluentValidation;

using RTSCore.Domain.Configurations.World;

namespace RTSCore.Application.Campaign.ArmyMovement;

public sealed class MoveArmyCommandValidator : AbstractValidator<MoveArmyCommand>
{
    public MoveArmyCommandValidator(WorldConfiguration worldConfig)
    {
        RuleFor(a => a.Id.Value).NotEmpty();
        RuleFor(a => a.X).InclusiveBetween(worldConfig.MinX, worldConfig.MaxX);
        RuleFor(a => a.Y).InclusiveBetween(worldConfig.MinY, worldConfig.MaxY);
    }
}