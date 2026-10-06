using MediatR;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects.AI;
using RTSCore.Domain.ValueObjects.Enums;

namespace RTSCore.Application.AI.Recruitment;

public class AiRecruiter(IUnitOfWork unitOfWork, AiRecruitOptionCache recruitOptions, IMediator mediator)
{
    public async Task Recruit(FactionType faction, AiPersonality personality, CancellationToken cancellationToken)
    {
        var armies = await unitOfWork.ArmyRepository.GetFactionArmiesAsync(faction, cancellationToken);
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