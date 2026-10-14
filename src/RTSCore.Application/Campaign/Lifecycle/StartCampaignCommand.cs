using MediatR;

using RTSCore.Domain;

namespace RTSCore.Application.Campaign.Lifecycle;

public record StartCampaignCommand(FactionType[] PlayersFactions) : IRequest;