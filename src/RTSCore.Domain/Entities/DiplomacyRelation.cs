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
    public bool IsWar { get; private set; }

    public const int MinStanding = -100;
    public const int MaxStanding = 100;

    public DiplomacyRelation(FactionType factionA, FactionType factionB, int startingStanding)
    {
        if (factionA == factionB)
        {
            throw new GameRuleException("Фракция не может установить дипломатические отношения сама с собой");
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
        if (IsWar)
        {
            throw new GameRuleException("Нельзя заключить торговый договор, фракции в состоянии войны");
        }

        if (HasTradeAgreement)
        {
            throw new GameRuleException("Торговый договор уже заключён");
        }

        if (Standing < GameBalance.Diplomacy.MinStandingForTrade)
        {
            throw new GameRuleException(
                $"Нельзя заключить торговый договор. Уровень отношений: {Standing}. " +
                $"Требуется минимум: {GameBalance.Diplomacy.MinStandingForTrade}"
            );
        }

        HasTradeAgreement = true;
        ChangeStanding(GameBalance.Diplomacy.AcceptTradeOfferBonus);
    }

    public void DeclareWare()
    {
        if (IsWar)
        {
            throw new GameRuleException("Нельзя объявить войну. Фракции уже воюют");
        }

        HasTradeAgreement = false;

        IsWar = true;
        ChangeStanding(GameBalance.Diplomacy.DeclareWarPenalty);
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
        Standing = Math.Clamp(Standing + amount, MinStanding, MaxStanding);
    }
}