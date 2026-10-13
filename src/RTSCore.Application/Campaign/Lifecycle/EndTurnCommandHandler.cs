using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Services.Lifecycle;

namespace RTSCore.Application.Campaign.Lifecycle;

public class EndTurnCommandHandler(
    IUnitOfWork unitOfWork,
    TurnEndService turnEndService
) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken ct)
    {
        var faction = await unitOfWork.FactionRepository.GetAsync(request.FactionType, ct);
        Guard.Against.NotFound(faction, request.FactionType);

        var cities = await unitOfWork.CityRepository.GetAsync(faction.Type, ct);
        var relations = await unitOfWork.DiplomacyRelationRepository.GetAsync(faction.Type, ct);

        var partners = relations
            .Select(r => r.FactionA == faction.Type ? r.FactionB : r.FactionA)
            .ToList();

        var partnerCityCounts = await unitOfWork.CityRepository.GetFactionToCityCountAsync(partners, ct);

        turnEndService.TurnEnd(faction, cities, relations, partnerCityCounts);

        await unitOfWork.SaveChangesAsync(ct);
    }
}