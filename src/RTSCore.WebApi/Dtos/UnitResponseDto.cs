using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Dtos;

public record struct UnitResponseDto(
    string Id,
    UnitType Type,
    FactionType Faction,
    int Health,
    int Damage,
    int Armor,
    int Level,
    int Experience
);