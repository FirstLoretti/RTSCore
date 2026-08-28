using MediatR;

using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Campaing.Commands;

public class EndTurnCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EndTurnCommand>
{
    public async Task Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var activeConstructions = await unitOfWork.BuildingRepository.GetUnderConstructionAsync(cancellationToken);

        foreach (var construction in activeConstructions)
        {
            construction.AdvanceConstruction();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}