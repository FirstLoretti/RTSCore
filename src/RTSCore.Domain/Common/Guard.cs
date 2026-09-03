namespace RTSCore.Domain.Common;

public static class Guard
{
    public static GuardMarker Against { get; } = new GuardMarker();
}

public class GuardMarker { }