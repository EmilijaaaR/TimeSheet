using Application.MediatorUserRequest;
using Application.RequestEntities;
using Application.ViewEntities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        
        [ProducesResponseType(typeof(IEnumerable<UserView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _mediator.Send(new GetAllUsersRequest());
            return Ok(users);
        }

        [HttpGet("{id}")]
        
        [ProducesResponseType(typeof(UserView), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _mediator.Send(new GetUserRequest { Id = id });
            return Ok(user);
        }

        [HttpPost]
        
        [ProducesResponseType(typeof(UserView), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(CreateUserRequest userRequest)
        {
            var user = await _mediator.Send(userRequest);
            return Ok(user);
        }

        [HttpPut("{id}")]
        
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, UserRequest userRequest)
        {
            var user = await _mediator.Send(new UpdateUserRequest
            {
                Id = id,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Username = userRequest.Username,
                Password = userRequest.Password
            });
            return Ok(user);
        }

        [HttpDelete("{id}")]
        
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteUserRequest { Id = id });
            return NoContent();
        }
    }
}

