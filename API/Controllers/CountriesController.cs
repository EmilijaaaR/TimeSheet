using Application.Services;
using Application.ViewEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _countryService;

        public CountriesController(CountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(IEnumerable<CountryView>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryService.GetAllAsync();
            return Ok(countries);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _countryService.GetByIdAsync(id);
            return Ok(country);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(string name)
        {
            var country = await _countryService.CreateAsync(name);
            return Ok(country);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, string name)
        {
            var updatedCountry = await _countryService.UpdateAsync(id, name);
            return Ok(updatedCountry);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _countryService.DeleteAsync(id);
            return NoContent();
        }
    }
}

