using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services;

public class ArmyCreationService(IReadOnlyCollection<UnitTemplate> unitTemplates) : IArmyCreationService
{
    public Army CreateArmy(City city)
    {
        var army = city.RaiseArmy(type =>
            unitTemplates.FirstOrDefault(u => u.Type == type)
            ?? throw new NotFoundException($"Шаблона с типом {type} не существует")
        );

        return army;
    }
}