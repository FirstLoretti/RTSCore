using RTSCore.Domain.Entities;

namespace RTSCore.Domain.Interfaces;

public interface IArmyCreationService
{
    Army CreateArmy(City city);
}