using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Services.Combat;

public interface IAutoBattleService
{
    BattleResult StartAutoBattle(Army attacker, Army defender);
}