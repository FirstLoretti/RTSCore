using RTSCore.Domain.ValueObjects;

Test test = new();
Console.WriteLine(test.UnitId.Value);

class Test
{
    public readonly UnitId UnitId = "5";

    public void Testing()
    {

    }
}
