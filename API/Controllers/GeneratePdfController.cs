using Application.RequestEntities;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratePdfController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly GeneratePdfService _pdfService;

        public GeneratePdfController(ReportService reportService, GeneratePdfService pdfService)
        {
            _reportService = reportService;
            _pdfService = pdfService;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GeneratePdfReportAsync(ReportRequest request)
        {
            var reportData = await _reportService.GenerateReportAsync(request);
            var pdfBytes = _pdfService.GeneratePdfReport(reportData);
            return File(pdfBytes, "application/pdf", "report.pdf");
        }
    }
}
