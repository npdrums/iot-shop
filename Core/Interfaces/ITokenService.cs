using Core.Entities.Identity;

namespace Core.Interfaces
{
    public interface IJwtService
    {
         string CreateToken(AppUser user);
    }
}