namespace Shared.Constants
{
    public static class ConstantResponseCode
    {
        // Generic
        public const string Success = "Success";
        public const string CreateSuccess = "CreateSuccess";
        public const string UpdateSuccess = "UpdateSuccess";
        public const string DeleteSuccess = "DeleteSuccess";
        public const string NotFound = "NotFound";
        public const string Duplicate = "Duplicate";
        public const string ValidationError = "ValidationError";
        public const string SystemError = "SystemError";

        // SMS-specific
        public const string SmsQueued = "SmsQueued";
        public const string SmsInvalidNumber = "SmsInvalidNumber";
        public const string SmsEmptyMessage = "SmsEmptyMessage";
        public const string SmsOperatorNotDetected = "SmsOperatorNotDetected";
        public const string SmsProviderNotConfigured = "SmsProviderNotConfigured";

        // Auth
        public const string AuthLoginSuccess = "AuthLoginSuccess";
        public const string AuthInvalidCredentials = "AuthInvalidCredentials";
        public const string AuthAccountInactive = "AuthAccountInactive";
        public const string AuthAccountLocked = "AuthAccountLocked";
        public const string AuthInvalidToken = "AuthInvalidToken";
        public const string AuthLogoutSuccess = "AuthLogoutSuccess";
        public const string AuthUnauthorized = "AuthUnauthorized";
        public const string AuthForbidden = "AuthForbidden";

        // Admin CRUD (non-parameterized, resolve cleanly via SetResponse)
        public const string Saved = "Saved";
        public const string RecordNotFound = "RecordNotFound";
        public const string RecordDuplicate = "RecordDuplicate";
        public const string RoleProtected = "RoleProtected";
    }
}
