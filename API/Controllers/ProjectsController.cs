using API.RequestEntities;
using Application.RequestEntities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("byClientId/{clientId}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProjectsByClientId(int clientId)
        {
            var projects = await _projectService.GetProjectsByClientIdAsync(clientId);
            return Ok(projects);
        }

        [HttpGet("byFilters")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProjectsByFilters(int pageNumber = 1, int pageSize = 10, char? letter = null, string searchTerm = null)
        {
            var projects = await _projectService.GetProjectsByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            return Ok(projects);
        }

        //[HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<ProjectView>), 200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> GetAll()
        //{
        //    var projects = await _projectService.GetAllAsync();
        //    return Ok(projects);
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(ProjectView), 200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var project = await _projectService.GetByIdAsync(id);
        //    return Ok(project);
        //}

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(ProjectRequest request)
        {
            var createdProject = await _projectService.CreateAsync(request);
            return Ok(createdProject);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(ProjectUpdateRequest request)
        {
            var updatedProject = await _projectService.UpdateAsync(request);
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteAsync(id);
            return NoContent();
        }
    }
}

