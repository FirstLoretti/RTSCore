using MediatR;

using RTSCore.Application.Common;
using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Units.Commands;

public class AddExperienceCommandHandler(IUnitRepository repository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddExperienceCommand, (int Level, int Experience)>
{
    public async Task<(int Level, int Experience)> Handle(
        AddExperienceCommand request,
        CancellationToken cancellationToken
    )
    {
        var unit = await repository.GetUnitAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Юнита {request.Id} нет в базе данных");

        unit.AddExperience(request.Amount);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return (unit.Level, unit.Experience);
    }
}