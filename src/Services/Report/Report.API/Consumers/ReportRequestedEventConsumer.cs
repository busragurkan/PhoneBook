using System.Net.Http.Json;
using MassTransit;
using PhoneBook.Shared.DTOs;
using PhoneBook.Shared.Events;
using Report.API.Entities;
using Report.API.Repositories;

namespace Report.API.Consumers;

public class ReportRequestedEventConsumer : IConsumer<ReportRequestedEvent>
{
    private readonly IReportRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ReportRequestedEventConsumer> _logger;

    public ReportRequestedEventConsumer(
        IReportRepository repository,
        IHttpClientFactory httpClientFactory,
        ILogger<ReportRequestedEventConsumer> logger)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        var reportId = context.Message.ReportId;
        var location = context.Message.RequestedLocation;

        _logger.LogInformation("Processing report {ReportId} for location: {Location}", reportId, location);

        try
        {
            // call Contact.API over REST to get location statistics
            var client = _httpClientFactory.CreateClient("ContactApi");
            var response = await client.GetAsync($"/api/contacts/statistics?location={Uri.EscapeDataString(location)}");
            response.EnsureSuccessStatusCode();

            var stats = await response.Content.ReadFromJsonAsync<LocationStatisticsDto>();

            var report = await _repository.GetByIdAsync(reportId);
            if (report == null)
            {
                _logger.LogWarning("Report {ReportId} not found, skipping", reportId);
                return;
            }

            report.Status = ReportStatus.Completed;
            report.ContactCount = stats?.ContactCount ?? 0;
            report.PhoneNumberCount = stats?.PhoneNumberCount ?? 0;
            report.CompletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(report);

            _logger.LogInformation("Report {ReportId} completed. Contacts: {Contacts}, Phones: {Phones}",
                reportId, report.ContactCount, report.PhoneNumberCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process report {ReportId}", reportId);
            throw; // let MassTransit handle retry
        }
    }
}
