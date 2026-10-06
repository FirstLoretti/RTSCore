using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Domain.ValueObjects.Common;

public readonly record struct BuildingEffect(BuildingEffectType Type, int Value);