using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Services.AiRecruiter;

public readonly record struct RecruitOption(UnitType Unit, int Cost, int Utility);