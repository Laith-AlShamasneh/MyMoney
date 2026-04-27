namespace Domain.Helpers;

public class SqlExceptionHelper(int errorNumber, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int ErrorNumber { get; } = errorNumber;
}
