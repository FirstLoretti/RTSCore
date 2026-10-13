using RTSCore.Domain.Enums;

namespace RTSCore.Application.AI.Recruitment;

public readonly record struct AiRecruitOption(UnitType Unit, int Cost, int Utility);