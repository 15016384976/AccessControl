using AccessControl.Architects.Services.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Threading.Tasks;

namespace AccessControl.Architects.Components.IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserService _userService;

        public ResourceOwnerPasswordValidator(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userService.FindByAccountPasswordAsync(context.UserName, context.Password);
            context.Result = user == null || user.Available == false ?
                new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid credentials") :
                new GrantValidationResult(user.Id.ToString(), "pwd", DateTime.UtcNow, _userService.AssembleClaims(user));
        }
    }
}
