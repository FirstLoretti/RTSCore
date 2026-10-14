using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.AI.Military;

public readonly record struct AiRecruitOption(UnitType Unit, int Cost, int Utility);