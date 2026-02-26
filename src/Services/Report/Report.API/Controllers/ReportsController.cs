using Microsoft.AspNetCore.Mvc;
using Report.API.DTOs;
using Report.API.Services;

namespace Report.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReportDto>>> GetAll()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportDto>> GetById(Guid id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
            return NotFound();

        return Ok(report);
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> RequestReport([FromBody] CreateReportRequestDto dto)
    {
        var report = await _reportService.RequestReportAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }
}
