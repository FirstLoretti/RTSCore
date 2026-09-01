using MediatR;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;

namespace RTSCore.Application.Campaing.Commands;

public class StartCampaignCommanHandler(
    IUnitOfWork unitOfWork,
    FactionPreset[] factionPresets
) : IRequestHandler<StartCampaignCommand>
{
    public async Task Handle(StartCampaignCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.FactionRepository.HasAnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Кампания уже запущена");
        }

        var factions = new List<Faction>();
        var cities = new List<City>();

        foreach (var faction in factionPresets)
        {
            var isHuman = request.SelectedFactions.Contains(faction.Type);
            var playerType = isHuman ? PlayerType.Human : PlayerType.Ai;

            factions.Add(new Faction(faction.Type, faction.Gold, playerType));

            foreach (var city in faction.Cities)
            {
                cities.Add(new City(city, faction.Type));
            }
        }

        unitOfWork.FactionRepository.AddRange(factions);
        unitOfWork.CityRepository.AddRange(cities);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}