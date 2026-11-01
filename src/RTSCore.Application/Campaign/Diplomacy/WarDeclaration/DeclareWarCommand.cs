using MediatR;

using RTSCore.Domain;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public sealed record DeclareWarCommand(CampaignId CampaignId, FactionType Initiator, FactionType Target) : IRequest;