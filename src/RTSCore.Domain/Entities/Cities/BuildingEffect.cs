using RTSCore.Domain.Enums;

namespace RTSCore.Domain.ValueObjects;

public readonly record struct BuildingEffect(BuildingEffectType Type, int Value);