using AccessControl.Architects.Services.Interfaces;
using AccessControl.Models.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessControl.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthorizeController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
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
