using FluentValidation;

using Microsoft.Extensions.Options;

namespace RTSCore.WebApi.Configurations;

public class FluentOptionsValidator<TOptions>(
    IValidator<TOptions> validator
) : IValidateOptions<TOptions> where TOptions : class
{
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        var result = validator.Validate(options);

        if (result.IsValid)
            return ValidateOptionsResult.Success;

        var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));

        return ValidateOptionsResult.Fail($"Ошибка валидации конфига игры");
    }
}