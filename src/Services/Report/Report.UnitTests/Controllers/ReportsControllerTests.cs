using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Report.API.Controllers;
using Report.API.DTOs;
using Report.API.Entities;
using Report.API.Services;

namespace Report.UnitTests.Controllers;

public class ReportsControllerTests
{
    private readonly Mock<IReportService> _serviceMock;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _serviceMock = new Mock<IReportService>();
        _controller = new ReportsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithReports()
    {
        // Arrange
        var reports = new List<ReportDto>
        {
            new() { Id = Guid.NewGuid(), RequestedLocation = "Ankara", Status = ReportStatus.Completed }
        };
        _serviceMock.Setup(s => s.GetAllReportsAsync()).ReturnsAsync(reports);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_WhenReportExists_ShouldReturnOk()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new ReportDto { Id = reportId, RequestedLocation = "Istanbul", Status = ReportStatus.Completed };
        _serviceMock.Setup(s => s.GetReportByIdAsync(reportId)).ReturnsAsync(report);

        // Act
        var result = await _controller.GetById(reportId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var returned = okResult!.Value as ReportDto;
        returned.Should().NotBeNull();
        returned!.RequestedLocation.Should().Be("Istanbul");
    }

    [Fact]
    public async Task GetById_WhenReportDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetReportByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ReportDto?)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task RequestReport_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = new CreateReportRequestDto { Location = "Ankara" };
        var report = new ReportDto { Id = Guid.NewGuid(), RequestedLocation = "Ankara", Status = ReportStatus.Preparing };
        _serviceMock.Setup(s => s.RequestReportAsync(dto)).ReturnsAsync(report);

        // Act
        var result = await _controller.RequestReport(dto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }
}
