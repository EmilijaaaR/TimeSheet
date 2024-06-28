using API.RequestEntities;
using Application.Services;
using Application.ViewEntities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LoginAndRegistrationController : Controller
    {
        private readonly UserService _userService;

        public LoginAndRegistrationController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(string username, string password)
        {
            var token = await _userService.AuthenticateAsync(username, password);
            return Ok(token);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserView), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(UserRequest userRequest)
        {
            var userView = await _userService.CreateAsync(userRequest);
            return Ok(userView);
        }
    }
}
