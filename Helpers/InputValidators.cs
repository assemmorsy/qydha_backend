
namespace Qydha.Helpers;


public static class InputValidators
{
    public static class Email
    {
        public const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string ErrorMessage = "Invalid email format.";
    }
    public static class Username
    {
        public const string Pattern = @"^[a-z0-9_.-]{3,20}$";
        public const string ErrorMessage = "Username must be 3 to 20 characters long and can contain letters, numbers, underscores, and periods.";
    }

    public static class OTPCode
    {
        public const string Pattern = @"^\d{6}$";
        public const string ErrorMessage = "OTP must be a 6-digit number.";

    }

    public static class GuidId
    {
        public const string Pattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
        public const string ErrorMessage = "Invalid GUID format.";

    }

    public static class Password
    {
        public const string Pattern = @"^(?=.*[a-zA-Z])(?=.*\d).{6,}$";
        public const string ErrorMessage = "Password must meet the strength criteria.";

    }

    public static class Phone
    {
        public const string Pattern = @"^\+\d{1,15}$";
        public const string ErrorMessage = "Phone number must start with a plus sign (+) followed by the Country Code then the phone number.";

    }

}
