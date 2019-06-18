using AccessControl.Architects.Services.Interfaces;
using AccessControl.Models.Authorize;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessControl.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly IUserService _userService;

        public AccountController(
            IConfiguration configuration,
            IIdentityServerInteractionService interactionService,
            IClientStore clientStore,
            IUserService userService)
        {
            _configuration = configuration;
            _interactionService = interactionService;
            _clientStore = clientStore;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string button)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button == "CANCEL")
            {
                if (context == null)
                {
                    return Redirect("~/");
                }
                else
                {
                    await _interactionService.GrantConsentAsync(context, ConsentResponse.Denied);
                    if (string.IsNullOrWhiteSpace(context.ClientId) == false)
                    {
                        var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                        if (client?.RequirePkce == true)
                        {
                            return View("Redirect", new RedirectModel { RedirectUrl = model.ReturnUrl });
                        }
                    }
                    return Redirect(model.ReturnUrl);
                }
            }

            if (string.IsNullOrEmpty(model.Account))
            {
                ModelState.AddModelError(string.Empty, "account should not be null");
            }
                
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(string.Empty, "password should not be null");
            }

            if (ModelState.IsValid)
            {
                var user = await _userService.FindByAccountPasswordAsync(model.Account, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "account or password incorrect");
                }
                else
                {
                    if (user.Available == false)
                    {
                        ModelState.AddModelError(string.Empty, "account not available");
                    }

                    AuthenticationProperties properties = null;
                    if (model.Remember)
                    {
                        properties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                        };
                    };

                    await HttpContext.SignInAsync(user.Id.ToString(), user.Account, properties);

                    if (context == null)
                    {
                        if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            throw new ArgumentException("return url incorrect");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(context.ClientId) == false)
                        {
                            var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                            if (client?.RequirePkce == true)
                            {
                                return View("Redirect", new RedirectModel { RedirectUrl = model.ReturnUrl });
                            }
                        }
                        return Redirect(model.ReturnUrl);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var model = new LogoutModel { LogoutId = logoutId };
            return await Logout(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            var context = await _interactionService.GetLogoutContextAsync(model.LogoutId);
            model.PostLogoutRedirectUri = context?.PostLogoutRedirectUri;
            if (User?.Identity.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signin([FromBody]SigninModel model)
        {
            if (model.Validate())
            {
                var user = await _userService.FindByAccountPasswordAsync(model.Account, model.Password);

                if (user == null)
                {
                    throw new ValidationException("account or password incorrect");
                }

                if (user.Available == false)
                {
                    throw new ValidationException("account not available");
                }

                var dict = new Dictionary<string, string>
                {
                    ["client_id"] = "client", //
                    ["client_secret"] = "secret", //
                    ["grant_type"] = "password",
                    ["username"] = model.Account,
                    ["password"] = model.Password
                };

                using (var client = new HttpClient())
                using (var content = new FormUrlEncodedContent(dict))
                {
                    var urls = _configuration["urls"] + "/connect/token";
                    var response = await client.PostAsync(_configuration["urls"] + "/connect/token", content);
                    var token = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    return Ok(token);
                }
            }

            return BadRequest();
        }
    }
}
