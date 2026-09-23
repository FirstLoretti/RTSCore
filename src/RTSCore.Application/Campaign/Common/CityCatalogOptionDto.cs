namespace RTSCore.Application.Campaign.Common;

public record CityCatalogOptionDto<T>(
    T Type,
    string DisplayName,
    int Cost,
    CityCatalogOptionAvailability Availability,
    string? LockReason = null
) where T : Enum;