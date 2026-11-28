using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Common;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed record EndTurnCommand(CampaignId CampaignId, FactionType Faction) : IRequest<Result>;