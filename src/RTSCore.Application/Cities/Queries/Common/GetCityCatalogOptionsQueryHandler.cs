using MediatR;

using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Cities.Queries.Common;

public abstract class GetCityCatalogOptionsQueryHandler<Query, T, Template>(
    IUnitOfWork unitOfWork,
    IReadOnlyCollection<Template> templates
) : IRequestHandler<Query, IReadOnlyCollection<CityCatalogOptionDto<T>>>
    where Query : IRequest<IReadOnlyCollection<CityCatalogOptionDto<T>>>, ICityQuery
    where T : Enum
    where Template : ICatalogOption<T>
{

    protected abstract bool IsVisibleInCity(Template template, City city);

    public async Task<IReadOnlyCollection<CityCatalogOptionDto<T>>> Handle(Query request, CancellationToken cancellationToken)
    {
        var cityId = request.CityId;
        var city = await unitOfWork.CityRepository.GetCityWithBuildingsAsync(cityId, cancellationToken)
            ?? throw new NotFoundException($"[{nameof(GetCityCatalogOptionsQueryHandler<,,>)}] Поселения {cityId} нет на карте");

        var faction = await unitOfWork.FactionRepository.GetFactionAsync(city.OwnerFaction, cancellationToken)
            ?? throw new NotFoundException($"[{nameof(GetCityCatalogOptionsQueryHandler<,,>)}] Фракции {city.OwnerFaction} нет на карте");

        var catalogOptions = new List<CityCatalogOptionDto<T>>();

        foreach (var template in templates)
        {
            if (!IsVisibleInCity(template, city)) continue;

            var hasEnoughGold = faction.Gold >= template.Cost;
            var availability = hasEnoughGold ? CityCatalogOptionAvailability.Available : CityCatalogOptionAvailability.Locked;
            var lockReason = !hasEnoughGold ? "Недостаточно средств" : null;

            catalogOptions.Add(new CityCatalogOptionDto<T>(
                template.Type, template.DisplayName, template.Cost, availability, lockReason));
        }

        return catalogOptions;
    }
}