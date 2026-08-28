using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class DeleteUnitCommandValidator : AbstractValidator<DeleteUnitCommand>
{
    public DeleteUnitCommandValidator()
    {
        RuleFor(e => e.Id).NotEmpty().Length(3, 30);
    }
}