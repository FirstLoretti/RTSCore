using MediatR;

using RTSCore.Domain.Common.Exceptions;
using RTSCore.Domain.Common.Repositories;

namespace RTSCore.Application.Campaign.Diplomacy.WarDeclaration;

public class DeclareWarCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeclareWarCommand>
{
    public async Task Handle(DeclareWarCommand request, CancellationToken cancellationToken)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetAsync(
            request.Initiator, request.Target, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, request.Initiator, request.Target);

        relation.DeclareWar();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}