namespace RTSCore.Domain.Exeptions;

public class NotFoundException(string message) : Exception(message);