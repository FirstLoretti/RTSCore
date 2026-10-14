using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI.Diplomacy;

public sealed record AiDiplomacyContext(
    AiFactionContext Faction,
    AiPersonality AiPersonality,
    IReadOnlyCollection<AiFactionContext> OpponentsFactions,
    IReadOnlyCollection<DiplomacyRelation> Relations,
    IReadOnlyCollection<DiplomacyOffer> IncomingOffers
)
{
    public DiplomacyRelation GetRelation(FactionType intiator)
        => Relations.First(r => r.FactionA == intiator || r.FactionB == intiator);
}