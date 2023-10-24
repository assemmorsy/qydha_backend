using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PhoneNumberAttribute : ValidationAttribute
{
    private readonly string[] patterns;

    public PhoneNumberAttribute()
    {
        var EgyptPattern = @"^(\+201)[0-2,5][0-9]{8}$";
        var SaudiArabiaPattern = @"^(\+9665)[0-1,3-9][0-9]{7}$";
        var IraqPattern = @"^(\+9647)[3-9][0-9]{8}$";
        var JordanPattern = @"^(\+9627)[5,7-9][0-9]{7}$";
        var BahrainPattern = @"^(\+9733)[1-4,6-9][0-9]{6}$";
        var UAEPattern = @"^(\+971)[2-9]\d{8}$";
        var QatarPattern = @"^(\+974)[3567]\d{7}$"; ;
        var KuwaitPattern = @"^(\+965)[569]\d{7}$";

        patterns = new string[] { EgyptPattern, SaudiArabiaPattern, IraqPattern, JordanPattern, BahrainPattern, UAEPattern, QatarPattern, KuwaitPattern };
    }

    public override bool IsValid(object? value)
    {
        if (value is not null && value is string phoneNumber)
        {
            foreach (var pattern in patterns)
            {
                Regex regex = new (pattern);
                if (regex.IsMatch(phoneNumber))
                {
                    return true;
                }
            }
        }
        return false;
    }
}