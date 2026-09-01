namespace RTSCore.Application.Cities.Queries;

public record CityCatalogOptionDto<T>(
    T Type,
    string DisplayName,
    int Cost,
    CityCatalogOptionAvailability Availability,
    string? LockReason = null
) where T : Enum;