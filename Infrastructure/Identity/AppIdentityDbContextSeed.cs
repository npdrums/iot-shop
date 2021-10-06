using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Nikola",
                    Email = "nikola@test.com",
                    UserName = "nikola@test.com",
                    Address = new Address
                    {
                        FirstName = "Nikola",
                        LastName = "Pujaz",
                        Street = "117A Bleecker Street",
                        City = "New York",
                        State = "NY",
                        Zipcode = "10012"
                    }
                };

                await userManager.CreateAsync(user, "NovaLozinka1#");
            }
        }
    }
}