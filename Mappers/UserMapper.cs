using Qydha.Entities;
using Qydha.Models;
using Riok.Mapperly.Abstractions;


namespace Qydha.Mappers;
[Mapper]
public partial class UserMapper
{
    [MapProperty(nameof(User.Created_On), nameof(GetUserDto.CreatedOn))]
    [MapProperty(nameof(User.Is_Email_Confirmed), nameof(GetUserDto.IsEmailConfirmed))]
    [MapProperty(nameof(User.Is_Phone_Confirmed), nameof(GetUserDto.IsPhoneConfirmed))]
    [MapProperty(nameof(User.Is_Anonymous), nameof(GetUserDto.IsAnonymous))]
    [MapProperty(nameof(User.Avatar_Url), nameof(GetUserDto.AvatarUrl))]
    [MapProperty(nameof(User.Birth_Date), nameof(GetUserDto.BirthDate))]
    [MapProperty(nameof(User.Last_Login), nameof(GetUserDto.LastLogin))]
    [MapProperty(nameof(User.Free_Subscription_Used), nameof(GetUserDto.FreeSubscriptionUsed))]
    [MapProperty(nameof(User.Expire_Date), nameof(GetUserDto.ExpireDate))]
    public partial GetUserDto UserToUserDto(User user);
}