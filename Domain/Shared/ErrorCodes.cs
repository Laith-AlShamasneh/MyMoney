namespace Domain.Shared;

public static class ErrorCodes
{
    // --- General & Operations ---
    public static class Common
    {
        public const string UNKNOWN = "COMMON_UNKNOWN";
        public const string VALIDATION_ERROR = "COMMON_VALIDATION_ERROR";
        public const string NOT_FOUND = "COMMON_RESOURCE_NOT_FOUND";
        public const string CONFLICT = "COMMON_CONFLICT";
        public const string OPERATION_FAILED = "COMMON_OPERATION_FAILED";
        public const string INVALID_INPUT = "COMMON_INVALID_INPUT";
        public const string UNAUTHORIZED = "COMMON_UNAUTHORIZED";
        public const string FORBIDDEN = "COMMON_FORBIDDEN";
        public const string INVALID_OPERATION = "COMMON_INVALID_OPERATION";
    }

    public static class System
    {
        public const string UNEXPECTED = "SYSTEM_UNEXPECTED_ERROR";
        public const string DB_INSERT_FAILED = "SYSTEM_DB_INSERT_FAILED";
        public const string DB_UPDATE_FAILED = "SYSTEM_DB_UPDATE_FAILED";
        public const string DB_DELETE_FAILED = "SYSTEM_DB_DELETE_FAILED";
    }

    // --- Identity & Access ---
    public static class Authentication
    {
        public const string INVALID_CREDENTIALS = "AUTH_INVALID_CREDENTIALS";
        public const string USER_NOT_FOUND = "AUTH_USER_NOT_FOUND";
        public const string ACCOUNT_LOCKED = "AUTH_ACCOUNT_LOCKED";
        public const string ACCOUNT_DISABLED = "AUTH_ACCOUNT_DISABLED";
        public const string EMAIL_ALREADY_EXISTS = "AUTH_EMAIL_ALREADY_EXISTS";
        public const string EMAIL_NOT_CONFIRMED = "AUTH_EMAIL_NOT_CONFIRMED";
        public const string TOKEN_INVALID = "AUTH_TOKEN_INVALID";
        public const string TOKEN_EXPIRED = "AUTH_TOKEN_EXPIRED";
    }

    public static class Authorization
    {
        public const string ROLE_REQUIRED = "AUTHZ_ROLE_REQUIRED";
        public const string PERMISSION_DENIED = "AUTHZ_PERMISSION_DENIED";
    }

    // --- Lookups & Geography ---
    public static class Lookups
    {
        public const string COUNTRY_NOT_FOUND = "LOOKUP_COUNTRY_NOT_FOUND";
        public const string CITY_NOT_FOUND = "LOOKUP_CITY_NOT_FOUND";
    }
}