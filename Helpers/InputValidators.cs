
namespace Qydha.Helpers;


public static class InputValidators
{

    public static class Name
    {
        public const string Pattern = @"^[A-Za-z\u0621-\u064A0-9\s]{4,}$";
        public const string ErrorMessage = "يجب ان يحتوى الاسم علي 4 حروف علي الاقل";
    }

    public static class Email
    {
        public const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string ErrorMessage = "برجاء ادخال بريد الكتروني صحيح";
    }
    public static class Username
    {
        public const string Pattern = @"^[a-z0-9_.-]{3,100}$";
        public const string ErrorMessage = "يجب ان يحتوي اسم المستخدم علي 3 حروف علي الاقل ";
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
        public const string ErrorMessage = "يجب ان تحتوى كلمة المرور علي 6 حروف علي الاقل";

    }

    public static class Phone
    {
        public const string ErrorMessage = "برجاء ادخال رقم جوال صحيح ويستخدم تطبيق الواتس اب";
    }

}
