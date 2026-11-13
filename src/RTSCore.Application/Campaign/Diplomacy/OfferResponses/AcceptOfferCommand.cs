using MediatR;

using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.OfferResponses;

public sealed record AcceptOfferCommand(DiplomacyOfferId OfferId) : IRequest<Result>;