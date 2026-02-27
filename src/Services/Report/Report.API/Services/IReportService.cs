using Report.API.DTOs;

namespace Report.API.Services;

public interface IReportService
{
    Task<List<ReportDto>> GetAllReportsAsync();
    Task<ReportDto?> GetReportByIdAsync(Guid id);
    Task<ReportDto> RequestReportAsync(CreateReportRequestDto dto);
}
