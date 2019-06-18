using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Controllers.Interfaces
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("Users")]
    public class UserController : ControllerBase
    {

    }
}
