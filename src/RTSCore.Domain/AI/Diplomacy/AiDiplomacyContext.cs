using System.Collections.Immutable;

using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Domain.AI.Diplomacy;

public sealed record AiDiplomacyContext(
    AiFactionContext Faction,
    AiPersonality AiPersonality,
    ImmutableArray<AiFactionContext> AnotherFactions,
    ImmutableArray<DiplomacyRelation> Relations,
    ImmutableArray<DiplomacyOffer> IncomingOffers
)
{
    public DiplomacyRelation GetRelation(FactionType faction)
        => Relations.First(r => r.FactionA == faction || r.FactionB == faction);
}