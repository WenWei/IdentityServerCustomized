﻿using IdentityServer4.Validation;
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

            context.Result = new GrantValidationResult(
                subject: userName,
                authenticationMethod: "",
                claims: new[] { new Claim("name", "whatever") });
        }
    }
}