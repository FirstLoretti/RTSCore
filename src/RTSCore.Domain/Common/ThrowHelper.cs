namespace RTSCore.Domain.Common;

public static class ThrowHelper
{
    public static Exception UnsupportedCategory<T>(T category) where T : Enum
    {
        return new ArgumentOutOfRangeException(
            nameof(category),
            category,
            $"Категория {typeof(T).Name}.{category} не поддерживается"
        );
    }
}