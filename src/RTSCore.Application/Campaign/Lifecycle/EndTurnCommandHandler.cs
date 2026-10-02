using MediatR;

using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.Services;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Campaign.Lifecycle;

public class EndTurnCommandHandler(
    IUnitOfWork unitOfWork,
    TurnEndService turnEndService
) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken ct)
    {
        var faction = await unitOfWork.FactionRepository.GetFactionAsync(request.FactionType, ct);
        Guard.Against.NotFound(faction, request.FactionType);

        var cities = await unitOfWork.CityRepository.GetCitiesAsync(faction.Type, ct);
        var relations = await unitOfWork.DiplomacyRelationRepository.GetDiplomacyRelations(faction.Type, ct);

        var partners = relations
            .Select(r => r.FactionA == faction.Type ? r.FactionB : r.FactionA)
            .ToList();

        var partnerCityCounts = new Dictionary<FactionType, int>();
        if (partners.Count > 0)
        {
            partnerCityCounts = await unitOfWork.CityRepository.GetFactionCityCounts(partners, ct);
        }

        turnEndService.TurnEnd(faction, cities, relations, partnerCityCounts);

        await unitOfWork.SaveChangesAsync(ct);
    }
}