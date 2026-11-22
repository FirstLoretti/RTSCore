using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;
using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Entities.Diplomacy;

public sealed class DiplomacyRelation
{
    public CampaignId CampaignId { get; }
    public DiplomacyRelationId Id { get; }
    public FactionType FactionA { get; }
    public FactionType FactionB { get; }

    public int Standing { get; private set; }
    public bool HasTradeAgreement { get; private set; }
    public bool InWar { get; private set; }

    public const int MaxStanding = 100;
    public const int MinStanding = -100;

    public DiplomacyRelation(CampaignId campaignId, FactionType factionA, FactionType factionB, int standing)
    {
        (FactionA, FactionB) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        CampaignId = campaignId;
        Id = DiplomacyRelationId.New();
        Standing = standing;
    }

    private DiplomacyRelation() { }

    public Result<DiplomacyOffer> CreateTradeOffer(FactionType initiator, FactionType target)
    {
        var result = ValidateFactions(initiator, target);
        if (result.IsFailure) return result.Error;

        var canTrade = CanTrade();
        if (canTrade.IsFailure) return canTrade.Error;

        return new DiplomacyOffer(CampaignId, initiator, target, OfferType.TradeAgreement);
    }

    public Result<DiplomacyOffer> CreatePeaceOffer(FactionType initiator, FactionType target)
    {
        var result = ValidateFactions(initiator, target);
        if (result.IsFailure) return result.Error;

        var canPeace = CanPeace();
        if (canPeace.IsFailure) return canPeace.Error;

        return new DiplomacyOffer(CampaignId, initiator, target, OfferType.PeaceTreaty);
    }

    public Result OpenTrade(int standingBonus)
    {
        var canTrade = CanTrade();
        if (canTrade.IsFailure) return canTrade;

        HasTradeAgreement = true;
        ChangeStanding(standingBonus);

        return Result.Success();
    }

    public Result DeclareWar(int standingPenalty)
    {
        if (InWar) return Result.Failure(GameErrors.CantWarFactionsAlreadyWar);

        HasTradeAgreement = false;
        InWar = true;
        ChangeStanding(standingPenalty);

        return Result.Success();
    }

    public Result MakePeace(int standingBonus)
    {
        var canPeace = CanPeace();
        if (canPeace.IsFailure) return canPeace;

        InWar = false;
        ChangeStanding(standingBonus);

        return Result.Success();
    }

    public Result CancelTrade(int standingPenalty)
    {
        if (!HasTradeAgreement) return Result.Failure(GameErrors.CantCancelTradeNotExist);

        HasTradeAgreement = false;
        ChangeStanding(standingPenalty);

        return Result.Success();
    }

    private void ChangeStanding(int amount) => Standing = int.Clamp(Standing + amount, MinStanding, MaxStanding);

    private Result CanTrade()
    {
        if (InWar) return Result.Failure(GameErrors.CantTradeWithEnemy);
        if (HasTradeAgreement) return Result.Failure(GameErrors.CantTradeAlreadyExist);

        return Result.Success();
    }

    private Result CanPeace()
    {
        if (!InWar) return Result.Failure(GameErrors.CantPeaceFactionsNotWar);

        return Result.Success();
    }

    private Result ValidateFactions(FactionType initiator, FactionType target)
    {
        return (initiator == FactionA && target == FactionB) || (target == FactionA && initiator == FactionB)
            ? Result.Success()
            : GameErrors.RelationNotContainFactions(initiator, target, Id);
    }
}