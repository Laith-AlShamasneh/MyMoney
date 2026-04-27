using Application.DTOs.Shared;
using Application.Helpers;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Application.Common.Helpers;

public static class ExecutionHelper
{
    /// <summary>
    /// Wraps a service execution in a Try/Catch block to ensure consistent error handling and logging.
    /// </summary>
    public static async Task<ServiceResponse<T>> ExecuteAsync<T>(
        Func<Task<ServiceResponse<T>>> action,
        ILogger logger,
        string operation,
        object? parameters = null,
        Languages lang = Languages.Ar,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        try
        {
            // 1. Check Cancellation before starting
            cancellationToken.ThrowIfCancellationRequested();

            // 2. Execute the actual business logic
            return await action();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // 4. Structured Logging for actual Errors
            logger.LogError(
                ex,
                "Failure in {Operation} | Method: {Member} | File: {File} | Line: {Line} | Params: {@Params}",
                operation,
                memberName,
                Path.GetFileName(filePath),
                lineNumber,
                parameters);

            // 5. Return a generic "System Error" to the client
            return ServiceResponse<T>.Failure(
                ErrorCodes.System.UNEXPECTED,
                MessagesHelper.GetMessage(MessageType.SystemError, lang),
                HttpResponseStatus.InternalServerError);
        }
    }
}