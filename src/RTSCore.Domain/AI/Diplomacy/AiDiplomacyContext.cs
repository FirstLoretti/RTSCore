using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI.Diplomacy;

public sealed record AiDiplomacyContext(
    AiFactionContext Faction,
    AiPersonality AiPersonality,
    IReadOnlyCollection<AiFactionContext> AnotherFactions,
    IReadOnlyCollection<DiplomacyRelation> FactionRelations,
    IReadOnlyCollection<DiplomacyOffer> IncomingOffers
)
{
    public DiplomacyRelation GetRelation(FactionType faction) =>
        FactionRelations.First(r => r.FactionA == faction || r.FactionB == faction);

    public AiFactionContext GetOpponent(FactionType opponent) =>
        AnotherFactions.First(f => f.FactionType == opponent);
}