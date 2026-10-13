using RTSCore.Domain.Errors;

namespace RTSCore.Domain.Common.Exceptions;

public class NotFoundException(GameError error) : Exception(error.Message);