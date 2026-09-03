using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class DiplomacyRelation
{
    public Guid Id { get; init; }
    public FactionType FactionA { get; init; }
    public FactionType FactionB { get; init; }
    public int Standing { get; private set; }
    public bool HasTradeAgreement { get; private set; }

    public const int MinRelationship = -100;
    public const int MaxRelationship = 100;

    public DiplomacyRelation(FactionType factionA, FactionType factionB, int startingStanding)
    {
        if (factionA == factionB)
        {
            throw new GameRuleException("Фракция не может установить дипломатические отношения сама с собой.");
        }

        Id = Guid.NewGuid();

        (FactionA, FactionB) = factionA < factionB
            ? (factionA, factionB)
            : (factionB, factionA);

        Standing = startingStanding;
    }

    private DiplomacyRelation() { }

    public void OpenTrade()
    {
        if (HasTradeAgreement)
        {
            throw new GameRuleException("Торговый договор уже заключён.");
        }

        if (Standing < GameBalance.Diplomacy.RequiredStandingForTrade)
        {
            throw new GameRuleException(
                $"Нельзя заключить торговый договор. Уровень отношений: {Standing}. " +
                $"Требуется минимум: {GameBalance.Diplomacy.RequiredStandingForTrade}"
            );
        }

        HasTradeAgreement = true;
        ChangeStanding(GameBalance.Diplomacy.TradeAgreementBonus);
    }

    public void CancelTrade()
    {
        if (!HasTradeAgreement)
        {
            throw new GameRuleException("Торговый договор не заключён");
        }

        HasTradeAgreement = false;
        ChangeStanding(GameBalance.Diplomacy.CancelTradePenalty);
    }

    public void ChangeStanding(int amount)
    {
        Standing = Math.Clamp(Standing + amount, MinRelationship, MaxRelationship);
    }
}