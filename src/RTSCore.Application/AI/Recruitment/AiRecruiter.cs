using MediatR;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.AI;

namespace RTSCore.Application.AI.Recruitment;

public class AiRecruiter(IUnitOfWork unitOfWork, RecruitOptionCache recruitOptions, IMediator mediator)
{
    public async Task Recruit(FactionType faction, AiPersonality personality, CancellationToken cancellationToken)
    {
        var armies = await unitOfWork.ArmyRepository.GetArmiesAsync(faction, cancellationToken);
        var options = recruitOptions.GetOptionFor(personality);

        foreach (var army in armies)
        {
            foreach (var option in options)
            {
                try
                {
                    await mediator.Send(new RecruitRegularUnitCommand(army.Id, option.Unit, faction), cancellationToken);
                }
                catch (GameRuleException) { continue; }
            }
        }
    }
}