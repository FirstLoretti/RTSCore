using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.AI.Recruitment;

public readonly record struct RecruitOption(UnitType Unit, int Cost, int Utility);