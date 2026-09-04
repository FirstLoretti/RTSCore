using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Common;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Campaing.Commands.Diplomacy;

public class DeclareWarCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeclareWarCommand>
{
    public async Task Handle(DeclareWarCommand request, CancellationToken cancellationToken)
    {
        var relation = await unitOfWork.DiplomacyRelationRepository.GetRelationAsync(
            request.Initiator, request.Target, cancellationToken
        );
        Guard.Against.NotFoundRelation(relation, request.Initiator, request.Target);

        relation.DeclareWare();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}