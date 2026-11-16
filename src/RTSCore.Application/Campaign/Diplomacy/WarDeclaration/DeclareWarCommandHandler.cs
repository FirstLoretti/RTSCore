using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Common;
using RTSCore.Domain.Configurations.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public sealed class DeclareWarCommandHandler(
    IUnitOfWork unitOfWork,
    DiplomacyConfiguration config
) : IRequestHandler<DeclareWarCommand, Result>
{
    public async Task<Result> Handle(DeclareWarCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        var result = relation.DeclareWar(config.DeclareWarPenalty);
        if (result.IsFailure) return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}