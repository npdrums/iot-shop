using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class User : IdentityUser
    {
        public string Nickname { get; set; }
        public UserAddress UserAddress { get; set; }
    }
}