namespace Domain.Helpers;

public class SqlExceptionHelper(int errorNumber, string message) : Exception(message)
{
    public int ErrorNumber { get; } = errorNumber;
}
