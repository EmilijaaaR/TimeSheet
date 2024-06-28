using API.RequestEntities;
using Application.RequestEntities;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetsController : ControllerBase
    {
        private readonly TimesheetService _timesheetService;

        public TimesheetsController(TimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        //[HttpGet("{id}")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var timesheet = await _timesheetService.GetByIdAsync(id);
        //    return Ok(timesheet);
        //}

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(Timesheet), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(int userId, DateTime date, IEnumerable<TimesheetRequest> timesheetRequest)
        {
            var result = await _timesheetService.CreateAsync(userId, date, timesheetRequest);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            await _timesheetService.DeleteAsync(id);
            return NoContent();
        }


        [HttpGet("status")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(typeof(TimesheetSummary), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTimesheetsWithStatus(int userId, DateTime startDate, DateTime endDate)
        {
            var summary = await _timesheetService.GetTimesheetsWithStatusAsync(userId, startDate, endDate);
            return Ok(summary);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateTimesheetsWithStatus(int userId, DateTime startDate, DateTime endDate, [FromBody] IEnumerable<TimesheetUpdateRequest> timesheetRequests)
        {
            var result = await _timesheetService.UpdateTimesheetsWithStatusAsync(userId, startDate, endDate,timesheetRequests);
            return Ok(result);
        }

    }
}

