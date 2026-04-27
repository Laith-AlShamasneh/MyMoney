using System.ComponentModel;

namespace Domain.Shared;

public enum Languages
{
    En = 1,
    Ar = 2
}

public enum SignInFailureReason
{
    UnknownError = 1,
    InvalidPassword = 2,
    UserNotFound = 3,
    UserNotActive = 4,
    UserLockedOut = 5,
    EmailNotConfirmed = 6,
    ClientIpBlacklisted = 7,
    CaptchaFailed = 8,
    TokenExpired = 9
}

public enum FileUploadType
{
    UserProfileImage = 1,
    Document = 2
}

public enum UserType
{
    Normal = 1,
    Admin = 2,
    SystemAdmin = 3
}

public enum HttpResponseStatus
{
    [Description("OK")] OK = 200,
    [Description("Created")] Created = 201,
    [Description("Bad Request")] BadRequest = 400,
    [Description("Unauthorized")] Unauthorized = 401,
    [Description("Forbidden")] Forbidden = 403,
    [Description("Not Found")] NotFound = 404,
    [Description("Conflict")] Conflict = 409,
    [Description("Internal Server Error")] InternalServerError = 500
}