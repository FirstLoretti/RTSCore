using RTSCore.Domain.Interfaces;

namespace RTSCore.Domain.Services;

public class ExperienceTable : IExperienceTable
{
    public int GetLevel(int experiece) => experiece switch
    {
        < 100 => 1,
        < 200 => 2,
        _ => 3
    };
}