using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;
using RTSCore.Domain.Entities.Factions;

namespace RTSCore.Domain.AI.Diplomacy;

public sealed record AiDiplomacyContext(
    FactionContext FactionContext,
    AiPersonality AiPersonality,
    IReadOnlyCollection<FactionContext> AnotherFactions,
    IReadOnlyCollection<DiplomacyRelation> FactionRelations,
    IReadOnlyCollection<DiplomacyOffer> IncomingOffers
)
{
    public DiplomacyRelation GetRelation(FactionType faction) =>
        FactionRelations.First(r => r.FactionA == faction || r.FactionB == faction);

    public FactionContext GetOpponent(FactionType opponent) =>
        AnotherFactions.First(f => f.FactionType == opponent);
}