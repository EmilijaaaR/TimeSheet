using API.RequestEntities;
using Application.Services;
using Application.ViewEntities;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(IEnumerable<UserView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserView), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(UserRequest userRequest)
        {
            var user = await _userService.CreateAsync(userRequest);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, UserRequest userRequest)
        {
            var user = await _userService.UpdateAsync(id, userRequest);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}

