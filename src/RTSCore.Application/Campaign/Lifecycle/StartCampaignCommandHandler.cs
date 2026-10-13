using MediatR;

using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Services.Lifecycle;
using RTSCore.Domain.ValueObjects.Identifiers;

namespace RTSCore.Application.Campaign.Lifecycle;

public class StartCampaignCommanHandler(
    IUnitOfWork unitOfWork,
    StartCampaignService campaignService
) : IRequestHandler<StartCampaignCommand>
{
    public async Task Handle(StartCampaignCommand request, CancellationToken ct)
    {
        var (factions, cities) = campaignService.InitializeWorld(CampaignId.New(), request.PlayersFactions);

        unitOfWork.FactionRepository.AddRange(factions);
        unitOfWork.CityRepository.AddRange(cities);

        await unitOfWork.SaveChangesAsync(ct);
    }
}