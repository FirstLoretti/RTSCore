using Ardalis.GuardClauses;

using MediatR;

using RTSCore.Application.Repositories;
using RTSCore.Domain.Configurations.Diplomacy;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public sealed class DeclareWarCommandHandler(
    IUnitOfWork unitOfWork,
    DiplomacyConfiguration config
) : IRequestHandler<DeclareWarCommand>
{
    public async Task Handle(DeclareWarCommand request, CancellationToken ct)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(request.CampaignId, request.Initiator, request.Target, ct);
        Guard.Against.NotFound(request.Initiator, relation);

        relation.DeclareWar(config.DeclareWarPenalty);

        await unitOfWork.SaveChangesAsync(ct);
    }
}