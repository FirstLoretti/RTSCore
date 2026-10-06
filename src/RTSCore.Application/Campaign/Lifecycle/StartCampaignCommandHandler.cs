using MediatR;

using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Services.Lifecycle;

namespace RTSCore.Application.Campaign.Lifecycle;

public class StartCampaignCommanHandler(
    IUnitOfWork unitOfWork,
    StartCampaignService startCampaignService
) : IRequestHandler<StartCampaignCommand>
{
    public async Task Handle(StartCampaignCommand request, CancellationToken ct)
    {
        if (await unitOfWork.FactionRepository.HasAnyAsync(ct))
            throw new InvalidOperationException("Кампания уже запущена");

        var (factions, cities) = startCampaignService.InitializeNewWorld(
            request.CampaignId,
            request.SelectedFactions
        );

        unitOfWork.FactionRepository.AddRange(factions);
        unitOfWork.CityRepository.AddRange(cities);

        await unitOfWork.SaveChangesAsync(ct);
    }
}