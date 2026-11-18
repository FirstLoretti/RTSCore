using Microsoft.EntityFrameworkCore;

namespace RTSCore.Infrastructure;

public static class QueryableExtensions
{
    public static IQueryable<T> WithTracking<T>(this IQueryable<T> query, bool isTracking) where T : class =>
        isTracking ? query : query.AsNoTracking();
}