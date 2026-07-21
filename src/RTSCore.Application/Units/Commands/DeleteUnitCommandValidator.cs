using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class DeleteUnitCommandValidator : AbstractValidator<DeleteUnitCommand>
{
    public DeleteUnitCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty()
                .WithMessage("Id не может быть пустым")
            .Length(3, 30)
                .WithMessage("Длина Id от должна быть от 3 до 30 символов");
    }
}