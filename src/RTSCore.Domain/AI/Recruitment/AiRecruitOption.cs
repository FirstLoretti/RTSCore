using RTSCore.Domain.Entities.Armies;

namespace RTSCore.Domain.AI.Recruitment;

public readonly record struct AiRecruitOption(UnitType UnitType, int Cost, int Utility);