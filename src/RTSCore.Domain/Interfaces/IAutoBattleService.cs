using RTSCore.Domain.Entities;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Interfaces;

public interface IAutoBattleService
{
    BattleResult StartAutoBattle(Army attacker, Army defender);
}