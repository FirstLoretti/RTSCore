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
) : IRequestHandler<StartCampaignCommand>
{
    public async Task Handle(StartCampaignCommand request, CancellationToken ct)
    {
        var result = WorldInitializer.Initialize(CampaignId.New(), request.PlayersFactions, factionCatalog, cityCatalog);

        unitOfWork.FactionRepository.AddRange(result.Factions);
        unitOfWork.CityRepository.AddRange(result.Cities);

        await unitOfWork.SaveChangesAsync(ct);
    }
}