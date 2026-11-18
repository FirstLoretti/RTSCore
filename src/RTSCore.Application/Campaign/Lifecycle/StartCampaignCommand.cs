using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed record StartCampaignCommand(HashSet<FactionType> PlayersFactions) : IRequest<CampaignId>;