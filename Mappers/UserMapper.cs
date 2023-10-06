using Qydha.Entities;
using Qydha.Models;
using Riok.Mapperly.Abstractions;

namespace Qydha.Mappers;
[Mapper]
public partial class UserMapper
{
    public partial GetUserDto UserToUserDto(User user);
}