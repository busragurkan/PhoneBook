using MassTransit;
using PhoneBook.Shared.Events;
using Report.API.DTOs;
using Report.API.Entities;
using Report.API.Repositories;

namespace Report.API.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReportService(IReportRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<List<ReportDto>> GetAllReportsAsync()
    {
        var reports = await _repository.GetAllAsync();
        return reports.Select(MapToDto).ToList();
    }

    public async Task<ReportDto?> GetReportByIdAsync(Guid id)
    {
        var report = await _repository.GetByIdAsync(id);
        return report == null ? null : MapToDto(report);
    }

    public async Task<ReportDto> RequestReportAsync(CreateReportRequestDto dto)
    {
        var report = new Entities.Report
        {
            Id = Guid.NewGuid(),
            RequestedLocation = dto.Location,
            Status = ReportStatus.Preparing,
            RequestedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(report);

        await _publishEndpoint.Publish(new ReportRequestedEvent
        {
            ReportId = created.Id,
            RequestedLocation = dto.Location,
            RequestedAt = created.RequestedAt
        });

        return MapToDto(created);
    }

    private static ReportDto MapToDto(Entities.Report report)
    {
        return new ReportDto
        {
            Id = report.Id,
            RequestedLocation = report.RequestedLocation,
            Status = report.Status,
            ContactCount = report.ContactCount,
            PhoneNumberCount = report.PhoneNumberCount,
            RequestedAt = report.RequestedAt,
            CompletedAt = report.CompletedAt
        };
    }
}
