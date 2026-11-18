using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Cities;
using RTSCore.Domain.Configurations.Factions;
using RTSCore.Domain.Entities.Campaigns;

namespace RTSCore.Application.Campaign.Lifecycle;

public sealed class StartCampaignCommanHandler(
    IUnitOfWork unitOfWork,
    FactionCatalog factionCatalog,
    CityCatalog cityCatalog
) : IRequestHandler<StartCampaignCommand, CampaignId>
{
    public async Task<CampaignId> Handle(StartCampaignCommand request, CancellationToken ct)
    {
        var result = WorldInitializer.Initialize(CampaignId.New(), request.PlayersFactions, factionCatalog, cityCatalog);

        unitOfWork.CampaignRepository.Add(result.Campaign);
        unitOfWork.FactionRepository.AddRange(result.Factions);
        unitOfWork.CityRepository.AddRange(result.Cities);
        unitOfWork.AiAgentRepository.AddRange(result.AiAgents);

        await unitOfWork.SaveChangesAsync(ct);
        return result.Campaign.Id;
    }
}