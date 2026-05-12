using Application.DTOs.Shared;
using Domain.Helpers;
using Domain.Shared;

namespace Application.Helpers;

internal class MapExceptionHelper
{
    internal static ServiceResponse<T> MapAuthException<T>(SqlExceptionHelper ex, Languages languages)
    {
        return ex.ErrorNumber switch
        {
            50001 => ServiceResponse<T>.Failure(
                        ErrorCodes.Authentication.EMAIL_ALREADY_EXISTS,
                        MessagesHelper.GetMessage(MessageType.EmailAlreadyExists, languages),
                        HttpResponseStatus.Conflict),
            _ => ServiceResponse<T>.Failure(
                        ErrorCodes.Common.UNKNOWN,
                        MessagesHelper.GetMessage(MessageType.SystemError, languages),
                        HttpResponseStatus.InternalServerError)
        };
    }

    internal static ServiceResponse<T> MapFinanceException<T>(SqlExceptionHelper ex, Languages languages)
    {
        return ex.ErrorNumber switch
        {
            50002 => ServiceResponse<T>.Failure(
                        ErrorCodes.Common.INVALID_INPUT,
                        MessagesHelper.GetMessage(MessageType.InvalidInput, languages),
                        HttpResponseStatus.BadRequest),
            _ => ServiceResponse<T>.Failure(
                        ErrorCodes.Common.UNKNOWN,
                        MessagesHelper.GetMessage(MessageType.SystemError, languages),
                        HttpResponseStatus.InternalServerError)
        };
    }
}
