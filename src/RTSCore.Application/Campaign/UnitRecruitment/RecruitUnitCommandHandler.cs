using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Armies;
using RTSCore.Domain.Entities.Armies;
using RTSCore.Domain.Entities.Cities;

namespace RTSCore.Application.Campaign.UnitRecruitment;

public sealed class RecruitUnitCommandHandler(
    IUnitOfWork unitOfWork,
    UnitCatalog catalog
) : IRequestHandler<RecruitUnitCommand, Result<UnitId>>
{
    public async Task<Result<UnitId>> Handle(RecruitUnitCommand request, CancellationToken ct)
    {
        var army = await unitOfWork.ArmyRepository.GetAsync(request.ArmyId, ct);
        Guard.Against.NotFound(request.ArmyId, army);

        var city = await unitOfWork.CityRepository.GetAsync(request.CityId, ct, false);
        Guard.Against.NotFound(request.CityId, city);

        var faction = await unitOfWork.FactionRepository.GetAsync(army.CampaignId, army.Faction, ct);
        Guard.Against.NotFound(army.Faction, faction);

        var template = catalog.GetTemplate(request.UnitType);

        var cityResult = city.CanRecruit(template);
        if (cityResult.IsFailure) return cityResult.Error;

        var armyResult = army.RecruitUnit(template);
        if (armyResult.IsFailure) return armyResult.Error;

        var factionResult = faction.SpendGold(template.Cost);
        if (factionResult.IsFailure) return factionResult.Error;

        await unitOfWork.SaveChangesAsync(ct);

        return armyResult;
    }

    private static void ThrowIfDifferentFactions(Army army, City city)
    {
        if (army.Faction != city.Faction)
            throw new InvalidOperationException("Cannot recruit unit of a different factions");
    }

    private static void ThrowIfDifferentCampaignId(Army army, City city)
    {
        if (army.CampaignId != city.CampaignId)
            throw new InvalidOperationException("Cannot recruit unit when army and city have different campaignId");
    }
}