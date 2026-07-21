using FluentValidation;

namespace RTSCore.Application.Units.Commands;

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
                .WithMessage("Id не может быть пустым")
            .Length(3, 30)
                .WithMessage("Длина Id должна быть от 3 до 30 символов");

        RuleFor(c => c.Faction)
            .IsInEnum()
                .WithMessage("Неверный тип фракции");

        RuleFor(c => c.Type)
            .IsInEnum()
                .WithMessage("Неверный тип юнита");
    }
}