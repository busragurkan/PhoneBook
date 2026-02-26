using Contact.API.Data;
using Contact.API.Entities;
using Contact.API.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Shared.Enums;

namespace Contact.UnitTests.Repositories;

public class ContactRepositoryTests : IDisposable
{
    private readonly ContactDbContext _context;
    private readonly ContactRepository _repository;

    public ContactRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ContactDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContactDbContext(options);
        _repository = new ContactRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddContactToDatabase()
    {
        // Arrange
        var contact = new API.Entities.Contact
        {
            Id = Guid.NewGuid(),
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento"
        };

        // Act
        var result = await _repository.CreateAsync(contact);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Ali");

        var dbContact = await _context.Contacts.FindAsync(contact.Id);
        dbContact.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllContacts()
    {
        // Arrange
        _context.Contacts.AddRange(
            new API.Entities.Contact { Id = Guid.NewGuid(), Name = "Ali", Surname = "Yilmaz", Company = "Arvento" },
            new API.Entities.Contact { Id = Guid.NewGuid(), Name = "Mehmet", Surname = "Demir", Company = "TechCorp" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnContactWithInformations()
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
                new() { Id = Guid.NewGuid(), ContactId = contactId, InfoType = ContactInfoType.PhoneNumber, InfoContent = "05551234567" }
            }
        };
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithDetailsAsync(contactId);

        // Assert
        result.Should().NotBeNull();
        result!.ContactInformations.Should().HaveCount(1);
        result.ContactInformations.First().InfoContent.Should().Be("05551234567");
    }

    [Fact]
    public async Task DeleteAsync_WhenContactExists_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _context.Contacts.Add(new API.Entities.Contact
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(contactId);

        // Assert
        result.Should().BeTrue();
        var deleted = await _context.Contacts.FindAsync(contactId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenContactDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddContactInformationAsync_ShouldAddInfoToDatabase()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _context.Contacts.Add(new API.Entities.Contact
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento"
        });
        await _context.SaveChangesAsync();

        var info = new ContactInformation
        {
            Id = Guid.NewGuid(),
            ContactId = contactId,
            InfoType = ContactInfoType.Email,
            InfoContent = "ali@example.com"
        };

        // Act
        var result = await _repository.AddContactInformationAsync(info);

        // Assert
        result.Should().NotBeNull();
        result.InfoContent.Should().Be("ali@example.com");
    }

    [Fact]
    public async Task RemoveContactInformationAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var infoId = Guid.NewGuid();
        _context.Contacts.Add(new API.Entities.Contact
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento"
        });
        _context.ContactInformations.Add(new ContactInformation
        {
            Id = infoId,
            ContactId = contactId,
            InfoType = ContactInfoType.PhoneNumber,
            InfoContent = "05551234567"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.RemoveContactInformationAsync(infoId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ContactExistsAsync_WhenContactExists_ShouldReturnTrue()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _context.Contacts.Add(new API.Entities.Contact
        {
            Id = contactId,
            Name = "Ali",
            Surname = "Yilmaz",
            Company = "Arvento"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ContactExistsAsync(contactId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ContactExistsAsync_WhenContactDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ContactExistsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
