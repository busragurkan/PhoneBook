using FluentAssertions;
using MassTransit;
using Moq;
using PhoneBook.Shared.Events;
using Report.API.DTOs;
using Report.API.Entities;
using Report.API.Repositories;
using Report.API.Services;

namespace Report.UnitTests.Services;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _repositoryMock = new Mock<IReportRepository>();
        _publishMock = new Mock<IPublishEndpoint>();
        _service = new ReportService(_repositoryMock.Object, _publishMock.Object);
    }

    [Fact]
    public async Task GetAllReportsAsync_ShouldReturnAllReports()
    {
        // Arrange
        var reports = new List<API.Entities.Report>
        {
            new() { Id = Guid.NewGuid(), RequestedLocation = "Ankara", Status = ReportStatus.Completed, ContactCount = 5, PhoneNumberCount = 8, RequestedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), RequestedLocation = "Istanbul", Status = ReportStatus.Preparing, RequestedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(reports);

        // Act
        var result = await _service.GetAllReportsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].RequestedLocation.Should().Be("Ankara");
    }

    [Fact]
    public async Task GetReportByIdAsync_WhenReportExists_ShouldReturnReport()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new API.Entities.Report
        {
            Id = reportId,
            RequestedLocation = "Istanbul",
            Status = ReportStatus.Completed,
            ContactCount = 10,
            PhoneNumberCount = 15,
            RequestedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);

        // Act
        var result = await _service.GetReportByIdAsync(reportId);

        // Assert
        result.Should().NotBeNull();
        result!.RequestedLocation.Should().Be("Istanbul");
        result.ContactCount.Should().Be(10);
    }

    [Fact]
    public async Task GetReportByIdAsync_WhenReportDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((API.Entities.Report?)null);

        // Act
        var result = await _service.GetReportByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RequestReportAsync_ShouldCreateReportAndPublishEvent()
    {
        // Arrange
        var dto = new CreateReportRequestDto { Location = "Ankara" };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<API.Entities.Report>()))
            .ReturnsAsync((API.Entities.Report r) => r);

        // Act
        var result = await _service.RequestReportAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.RequestedLocation.Should().Be("Ankara");
        result.Status.Should().Be(ReportStatus.Preparing);
        result.Id.Should().NotBeEmpty();

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<API.Entities.Report>()), Times.Once);
        _publishMock.Verify(p => p.Publish(
            It.Is<ReportRequestedEvent>(e => e.RequestedLocation == "Ankara"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestReportAsync_ShouldSetStatusToPreparing()
    {
        // Arrange
        var dto = new CreateReportRequestDto { Location = "Izmir" };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<API.Entities.Report>()))
            .ReturnsAsync((API.Entities.Report r) => r);

        // Act
        var result = await _service.RequestReportAsync(dto);

        // Assert
        result.Status.Should().Be(ReportStatus.Preparing);
        result.CompletedAt.Should().BeNull();
    }
}
