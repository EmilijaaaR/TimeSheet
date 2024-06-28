using API.RequestEntities;
using API.ViewEntities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("byFilters")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(IEnumerable<ClientView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetClientsByFilters([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] char? letter = null, [FromQuery] string searchTerm = null)
        {
            var clients = await _clientService.GetClientsByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            return Ok(clients);
        }

        [HttpGet("byUserId/{userId}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(IEnumerable<ClientView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var clients = await _clientService.GetClientsByUserIdAsync(userId);
            return Ok(clients);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(IEnumerable<ClientView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(ClientView), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            return Ok(client);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(ClientRequest request)
        {
            var createdClient = await _clientService.CreateAsync(request);
            return Ok(createdClient);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, ClientRequest request)
        {
            var updatedClient = await _clientService.UpdateAsync(id, request);
            return Ok(updatedClient);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _clientService.DeleteAsync(id);
            return NoContent();
        }
    }
}

