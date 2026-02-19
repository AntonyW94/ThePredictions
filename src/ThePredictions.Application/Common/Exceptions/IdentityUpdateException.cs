namespace ThePredictions.Application.Common.Exceptions;

public class IdentityUpdateException(IEnumerable<string> errors) : Exception("One or more Identity errors occurred.")
{
    public IEnumerable<string> Errors { get; } = errors;
}