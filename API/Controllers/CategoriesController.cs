using Application.Services;
using Application.ViewEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CategoryView), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(string name)
        {
            var category = await _categoryService.CreateAsync(name);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, string name)
        {
            return Ok(await _categoryService.UpdateAsync(id, name));
            
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();

        }

        [HttpGet("byFilters")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCategoriesByFilters(int pageNumber = 1, int pageSize = 10, char? letter = null, string searchTerm = null)
        {
            var categories = await _categoryService.GetCategoriesByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            return Ok(categories);
        }
    }
}
