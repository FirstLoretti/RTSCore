using MediatR;

using RTSCore.Domain.Interfaces;

namespace RTSCore.Application.Units.Commands;

public class AddExperienceCommandHandler(IUnitRepository repository) :
    IRequestHandler<AddExperienceCommand, (int Level, int Experience)>
{
    public Task<(int Level, int Experience)> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
    {
        var unit = repository.GetUnit(request.Id)
            ?? throw new KeyNotFoundException($"Юнита {request.Id} нет в базе данных");

        unit.AddExperience(request.Amount);

        repository.Save(unit);

        return Task.FromResult((unit.Level, unit.Experience));
    }
}