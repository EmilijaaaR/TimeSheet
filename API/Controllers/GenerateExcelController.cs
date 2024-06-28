using Application.RequestEntities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerateExcelController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly GenerateExcelService _excelService;

        public GenerateExcelController(ReportService reportService, GenerateExcelService excelService)
        {
            _reportService = reportService;
            _excelService = excelService;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateReportAsync(ReportRequest request)
        {
            var reportData = await _reportService.GenerateReportAsync(request);
            var excelBytes = _excelService.GenerateExcelReport(reportData);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }
    }
}
