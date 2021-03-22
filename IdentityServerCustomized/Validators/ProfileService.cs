using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace IdentityServerCustomized.Validators
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> logger;

        public ProfileService(ILogger<ProfileService> logger)
        {
            this.logger = logger;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;

            logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);

            var claims = new List<Claim>
            {
                // 必須有這 ClaimTypes.Name，在 WebAPI 的 HttpContext.User.Identity.Name 才會有值
                new Claim(ClaimTypes.Name, context.Subject.GetSubjectId()),
                new Claim(ClaimTypes.NameIdentifier, context.Subject.GetSubjectId())
            };

            claims.Add(context.Subject.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //var user = _userManager.GetUserAsync(context.Subject).Result;
            //context.IsActive = (user != null) && ((!user.LockoutEnd.HasValue) || (user.LockoutEnd.Value <= DateTime.Now));
            //return Task.FromResult(0);

        }
    }
}
