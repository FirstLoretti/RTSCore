using MediatR;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaing.Commands;

public record StartCampaignCommand(FactionType[] SelectedFactions) : IRequest;