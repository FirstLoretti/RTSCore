using System.Diagnostics.CodeAnalysis;

using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Common.Exceptions;

public static class Guard
{
    public static void NotFound<T>([NotNull] T? entity, Guid id) where T : class
    {
        if (entity == null)
            throw new NotFoundException(GameErrors.EntityNotFound(id));
    }

    public static void NotFound<T, TEnum>([NotNull] T? entity, TEnum type)
        where T : class
        where TEnum : Enum
    {
        if (entity == null)
            throw new NotFoundException(GameErrors.EntityNotFound(type));
    }

}