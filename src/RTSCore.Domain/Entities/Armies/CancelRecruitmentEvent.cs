using RTSCore.Domain.Common;

namespace RTSCore.Domain.Entities.Armies;

public record CancelRecruitmentEvent(FactionType Faction, int Cost) : IDomainEvent;