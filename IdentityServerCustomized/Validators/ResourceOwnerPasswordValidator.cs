using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerCustomized.Validators
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userName = context.UserName;
            var password = context.Password;

            var role = context.Request.Raw["role"] ?? "user";

            context.Result = new GrantValidationResult(
                subject: userName,
                authenticationMethod: "password",
                claims: new[] { 
                    new Claim(ClaimTypes.Role, role) ,
                    new Claim(ClaimTypes.Name, context.UserName),
                    new Claim(ClaimTypes.NameIdentifier, context.UserName)
                    }
                );
        }
    }
}
