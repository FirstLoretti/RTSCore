using MediatR;

using RTSCore.Domain;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed record StartCampaignCommand(HashSet<FactionType> PlayersFactions) : IRequest;