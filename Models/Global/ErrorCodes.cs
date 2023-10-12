namespace Qydha.Models;

public static class ErrorCodes
{
    public const string InvalidInput = "4000";
    public const string InvalidBodyInput = "4001";
    public const string InvalidPhotoFileInput = "4002";

    public const string NotFound = "4040";
    public const string AnonymousUserNotFound = "4041";
    public const string UserNotFound = "4042";
    public const string RegistrationRequestNotFound = "4043";
    public const string InvalidIAPHupToken = "4092";

    public const string EmailSendingError = "4093";

    public const string InvalidToken = "4094";

    public const string InvalidOTP = "4095";
    public const string OTPExceededTimeLimit = "4096";
    public const string InvalidCredentials = "4097";
    public const string AnonymousUserTokenNotProvided = "4098";

    public const string OTPSendingError = "4099";

    public const string DbUniqueViolation = "4100";
    public const string FileUploadError = "4101";
    public const string FileDeleteError = "4102";


    public const string ServerError = "5000";
    public const string UnknownError = "1000";

}
