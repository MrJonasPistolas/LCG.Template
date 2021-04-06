using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Auth;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LCG.Template.Server.Controllers.v1
{
    /// <summary>
    /// Login Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Method to Log in user into the system
        /// </summary>
        /// <param name="login">Login Model</param>
        /// <returns>Login result</returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if(ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(login);
                return result.Successful ? Ok(result) : BadRequest(result);
            }
            return BadRequest(new LoginResult { Successful = false, Error = "Error Login Model State Invalid..." });
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Cenas Maradas");
        }
    }
}
