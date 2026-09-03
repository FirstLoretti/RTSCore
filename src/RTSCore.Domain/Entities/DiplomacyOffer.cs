using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class DiplomacyOffer
{
    public Guid Id { get; init; }
    public FactionType Initiator { get; init; }
    public FactionType Target { get; init; }
    public OfferType Type { get; init; }
    public OfferStatus Status { get; private set; }

    public DiplomacyOffer(FactionType initiator, FactionType target, OfferType type)
    {
        if (initiator == target)
        {
            throw new GameRuleException("Нельзя отправить предложение самому себе.");
        }

        Id = Guid.NewGuid();
        Initiator = initiator;
        Target = target;
        Type = type;
        Status = OfferStatus.Pending;
    }

    private DiplomacyOffer() { }

    public void Accept()
    {
        if (Status != OfferStatus.Pending)
        {
            throw new GameRuleException($"[{nameof(DiplomacyOffer)}] Нельзя принять предложение со статусом {Status}");
        }

        Status = OfferStatus.Accepted;
    }

    public void Reject() => Status = OfferStatus.Rejeсted;
}