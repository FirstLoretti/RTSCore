using RTSCore.Domain;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy;

public readonly record struct DiplomacyOfferDto(DiplomacyOfferId OfferId, FactionType Initiator, OfferType OfferType);