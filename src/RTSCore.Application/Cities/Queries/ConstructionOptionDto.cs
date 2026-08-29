using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Cities.Queries;

public record ConstructionOptionDto(
    BuildingType Type,
    string DisplayName,
    int Cost,
    ConstructionOptionAvailability Availability,
    string? LockReason
);