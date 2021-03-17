using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServerCustomized.Validators
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //context.IsActive = false;
        }
    }
}
