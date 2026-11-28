using RTSCore.Domain.Common;
using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Entities.Campaigns;

public sealed class Campaign
{
    public CampaignId Id { get; }
    public FactionType FactionTurn { get; private set; }
    public int TurnNumber { get; private set; }
    public int PlayersCount { get; private set; }

    private readonly List<FactionType> _factions = [];
    public IReadOnlyList<FactionType> Factions => _factions;

    public bool IsAiTurn => _factions.IndexOf(FactionTurn) >= PlayersCount;

    internal Campaign(CampaignId id, IEnumerable<FactionType> factions, IReadOnlyCollection<FactionType> playersFactions)
    {
        Id = id;
        PlayersCount = playersFactions.Count;
        TurnNumber = 1;
        _factions = [.. factions
            .OrderBy(f => !playersFactions.Contains(f))
            .ThenBy(f => f.ToString())
        ];
        FactionTurn = _factions[0];
    }

    private Campaign() { }

    public Result AdvanceTurn(FactionType factionType)
    {
        if (factionType != FactionTurn) return GameErrors.NotYourTurn;

        var currentIndex = _factions.IndexOf(FactionTurn);
        var nextIndex = currentIndex + 1;

        if (nextIndex >= _factions.Count)
        {
            nextIndex = 0;
            TurnNumber++;
        }

        FactionTurn = _factions[nextIndex];

        return Result.Success();
    }
}