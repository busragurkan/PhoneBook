using Contact.API.DTOs;
using Contact.API.Entities;
using Contact.API.Repositories;
using Contact.API.Services;
using FluentAssertions;
using Moq;
using PhoneBook.Shared.Enums;

namespace Contact.UnitTests.Services;

public class ContactServiceTests
{
    private readonly Mock<IContactRepository> _repositoryMock;
    private readonly ContactService _service;

    public ContactServiceTests()
    {
        _repositoryMock = new Mock<IContactRepository>();
        _service = new ContactService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllContactsAsync_ShouldReturnAllContacts()
    {
        // Arrange
        var contacts = new List<API.Entities.Contact>
        {
            new() { Id = Guid.NewGuid(), Name = "Ali", Surname = "Yilmaz", Company = "Arvento" },
            new() { Id = Guid.NewGuid(), Name = "Mehmet", Surname = "Demir", Company = "Arvento" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(contacts);

        // Act
        var result = await _service.GetAllContactsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Ali");
        result[1].Name.Should().Be("Mehmet");
    }

    [Fact]
    public async Task GetAllContactsAsync_WhenNoContacts_ShouldReturnEmptyList()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<API.Entities.Contact>());

        // Act
        var result = await _service.GetAllContactsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetContactDetailAsync_WhenContactExists_ShouldReturnContactWithInformations()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new API.Entities.Contact
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento",
            ContactInformations = new List<ContactInformation>
            {
                new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.PhoneNumber, InfoContent = "05551234567" },
                new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.Email, InfoContent = "ali@example.com" }
            }
        };
        _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(contactId)).ReturnsAsync(contact);

        // Act
        var result = await _service.GetContactDetailAsync(contactId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Ali");
        result.Surname.Should().Be("Yilmaz");
        result.ContactInformations.Should().HaveCount(2);
        result.ContactInformations[0].InfoType.Should().Be(ContactInfoType.PhoneNumber);
    }

    [Fact]
    public async Task GetContactDetailAsync_WhenContactDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(contactId)).ReturnsAsync((API.Entities.Contact?)null);

        // Act
        var result = await _service.GetContactDetailAsync(contactId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateContactAsync_ShouldCreateAndReturnContact()
    {
        // Arrange
        var dto = new CreateContactDto
        {
            Name = "Ayse",
            Surname = "Kaya",
            Company = "TechCorp"
        };

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<API.Entities.Contact>()))
            .ReturnsAsync((API.Entities.Contact c) => c);

        // Act
        var result = await _service.CreateContactAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Ayse");
        result.Surname.Should().Be("Kaya");
        result.Company.Should().Be("TechCorp");
        result.Id.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<API.Entities.Contact>()), Times.Once);
    }

    [Fact]
    public async Task DeleteContactAsync_WhenContactExists_ShouldReturnTrue()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeleteAsync(contactId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteContactAsync(contactId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteContactAsync_WhenContactDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeleteAsync(contactId)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteContactAsync(contactId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddContactInformationAsync_WhenContactExists_ShouldReturnContactInformation()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var dto = new CreateContactInformationDto
        {
            InfoType = ContactInfoType.PhoneNumber,
            InfoContent = "05551234567"
        };

        _repositoryMock.Setup(r => r.ContactExistsAsync(contactId)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.AddContactInformationAsync(It.IsAny<ContactInformation>()))
            .ReturnsAsync((ContactInformation ci) => ci);

        // Act
        var result = await _service.AddContactInformationAsync(contactId, dto);

        // Assert
        result.Should().NotBeNull();
        result!.InfoType.Should().Be(ContactInfoType.PhoneNumber);
        result.InfoContent.Should().Be("05551234567");
    }

    [Fact]
    public async Task AddContactInformationAsync_WhenContactDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var dto = new CreateContactInformationDto
        {
            InfoType = ContactInfoType.Email,
            InfoContent = "test@example.com"
        };

        _repositoryMock.Setup(r => r.ContactExistsAsync(contactId)).ReturnsAsync(false);

        // Act
        var result = await _service.AddContactInformationAsync(contactId, dto);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(r => r.AddContactInformationAsync(It.IsAny<ContactInformation>()), Times.Never);
    }

    [Fact]
    public async Task RemoveContactInformationAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.RemoveContactInformationAsync(infoId)).ReturnsAsync(true);

        // Act
        var result = await _service.RemoveContactInformationAsync(infoId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveContactInformationAsync_WhenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var infoId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.RemoveContactInformationAsync(infoId)).ReturnsAsync(false);

        // Act
        var result = await _service.RemoveContactInformationAsync(infoId);

        // Assert
        result.Should().BeFalse();
    }
}
