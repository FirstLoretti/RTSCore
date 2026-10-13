using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services.Combat;

public interface IAutoBattleCalculator
{
    BattleResult Calculate(IReadOnlyCollection<Unit> attakers, IReadOnlyCollection<Unit> defenders);
}