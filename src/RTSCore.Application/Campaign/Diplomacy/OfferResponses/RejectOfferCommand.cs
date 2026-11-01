using MediatR;

using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public sealed record RejectOfferCommand(DiplomacyOfferId OfferId) : IRequest;