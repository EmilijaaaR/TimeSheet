using Application.RequestEntities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateReportAsync([FromBody] ReportRequest request)
        {
                var reportData = await _reportService.GenerateReportAsync(request);
                return Ok(reportData);
        }
    }
}
