using MediatR;

using RTSCore.Application.Campaign.UnitRecruitment;
using RTSCore.Domain.AI.Personality;
using RTSCore.Domain.Common.Repositories;
using RTSCore.Domain.Enums;
using RTSCore.Domain.ValueObjects.Exeptions;

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