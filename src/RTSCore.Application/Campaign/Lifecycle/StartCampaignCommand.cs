using MediatR;

using RTSCore.Domain.ValueObjects.Enums;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.Lifecycle;

public record StartCampaignCommand(
    CampaignId CampaignId,
    FactionType[] SelectedFactions
) : IRequest;