using System.Diagnostics.CodeAnalysis;

using RTSCore.Domain.Common;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Application.Common;

public static class ApplicationExtensions
{
    public static void NotFound<T>(
        this GuardMarker _,
        [NotNull] T? entity,
        object entityId
    ) where T : class
    {
        if (entity == null)
        {
            throw new NotFoundException($"Сущность {typeof(T).Name} с ID '{entityId}' не найдена");
        }
    }

    public static void NullRelation(
        this GuardMarker _,
        [NotNull] DiplomacyRelation? relation,
        FactionType a,
        FactionType b
    )
    {
        if (relation == null)
        {
            throw new NotFoundException($"[DiplomacyAi] Дипломатическая связь между {a} и {b} не установлена");
        }
    }

    public static void DuplicateOffer(this GuardMarker _, HashSet<FactionType> underNegotiation, FactionType target)
    {
        if (underNegotiation.Contains(target))
        {
            throw new GameRuleException($"Фракция {target} уже в процессе переговоров");
        }
    }

    public static void AlreadyTraded(this GuardMarker _, DiplomacyRelation relation)
    {
        if (relation.HasTradeAgreement)
        {
            throw new GameRuleException($"Фракции {relation.FactionA} и {relation.FactionB} уже имеют торговый договор");
        }
    }
}