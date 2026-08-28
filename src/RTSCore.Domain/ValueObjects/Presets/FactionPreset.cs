namespace RTSCore.Domain.ValueObjects.Presets;

public record FactionPreset(
    FactionType Type,
    int Gold,
    CityPreset[] Cities
);