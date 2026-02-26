using Contact.API.Controllers;
using Contact.API.DTOs;
using Contact.API.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneBook.Shared.Enums;

namespace Contact.UnitTests.Controllers;

public class ContactsControllerTests
{
    private readonly Mock<IContactService> _serviceMock;
    private readonly ContactsController _controller;

    public ContactsControllerTests()
    {
        _serviceMock = new Mock<IContactService>();
        _controller = new ContactsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithContacts()
    {
        // Arrange
        var contacts = new List<ContactDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Ali", Surname = "Yilmaz", Company = "Arvento" }
        };
        _serviceMock.Setup(s => s.GetAllContactsAsync()).ReturnsAsync(contacts);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var returnedContacts = okResult.Value as List<ContactDto>;
        returnedContacts.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_WhenContactExists_ShouldReturnOkWithContactDetail()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contactDetail = new ContactDetailDto
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento",
            ContactInformations = new List<ContactInformationDto>
            {
                new() { Id = Guid.NewGuid(), InfoType = ContactInfoType.PhoneNumber, InfoContent = "05551234567" }
            }
        };
        _serviceMock.Setup(s => s.GetContactDetailAsync(contactId)).ReturnsAsync(contactDetail);

        // Act
        var result = await _controller.GetById(contactId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var returned = okResult!.Value as ContactDetailDto;
        returned.Should().NotBeNull();
        returned!.Name.Should().Be("Ali");
        returned.ContactInformations.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_WhenContactDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetContactDetailAsync(contactId)).ReturnsAsync((ContactDetailDto?)null);

        // Act
        var result = await _controller.GetById(contactId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = new CreateContactDto { Name = "Ayse", Surname = "Kaya", Company = "TechCorp" };
        var created = new ContactDto { Id = Guid.NewGuid(), Name = "Ayse", Surname = "Kaya", Company = "TechCorp" };
        _serviceMock.Setup(s => s.CreateContactAsync(dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var returnedContact = createdResult.Value as ContactDto;
        returnedContact.Should().NotBeNull();
        returnedContact!.Name.Should().Be("Ayse");
    }

    [Fact]
    public async Task Delete_WhenContactExists_ShouldReturnNoContent()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteContactAsync(contactId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(contactId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WhenContactDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteContactAsync(contactId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(contactId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddContactInformation_WhenContactExists_ShouldReturnCreated()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var dto = new CreateContactInformationDto { InfoType = ContactInfoType.Email, InfoContent = "ali@example.com" };
        var created = new ContactInformationDto { Id = Guid.NewGuid(), InfoType = ContactInfoType.Email, InfoContent = "ali@example.com" };

        _serviceMock.Setup(s => s.AddContactInformationAsync(contactId, dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.AddContactInformation(contactId, dto);

        // Assert
        var createdResult = result.Result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task AddContactInformation_WhenContactDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var dto = new CreateContactInformationDto { InfoType = ContactInfoType.Email, InfoContent = "test@example.com" };
        _serviceMock.Setup(s => s.AddContactInformationAsync(contactId, dto)).ReturnsAsync((ContactInformationDto?)null);

        // Act
        var result = await _controller.AddContactInformation(contactId, dto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task RemoveContactInformation_WhenExists_ShouldReturnNoContent()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        _serviceMock.Setup(s => s.RemoveContactInformationAsync(infoId)).ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveContactInformation(infoId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveContactInformation_WhenDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        _serviceMock.Setup(s => s.RemoveContactInformationAsync(infoId)).ReturnsAsync(false);

        // Act
        var result = await _controller.RemoveContactInformation(infoId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
