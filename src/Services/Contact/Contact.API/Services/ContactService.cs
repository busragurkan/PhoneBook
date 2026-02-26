using Contact.API.DTOs;
using Contact.API.Entities;
using Contact.API.Repositories;
using PhoneBook.Shared.DTOs;
using PhoneBook.Shared.Enums;

namespace Contact.API.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContactDto>> GetAllContactsAsync()
    {
        var contacts = await _repository.GetAllAsync();

        return contacts.Select(c => new ContactDto
        {
            Id = c.Id,
            Name = c.Name,
            Surname = c.Surname,
            Company = c.Company
        }).ToList();
    }

    public async Task<ContactDetailDto?> GetContactDetailAsync(Guid id)
    {
        var contact = await _repository.GetByIdWithDetailsAsync(id);
        if (contact == null)
            return null;

        return new ContactDetailDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Surname = contact.Surname,
            Company = contact.Company,
            ContactInformations = contact.ContactInformations.Select(ci => new ContactInformationDto
            {
                Id = ci.Id,
                InfoType = ci.InfoType,
                InfoContent = ci.InfoContent
            }).ToList()
        };
    }

    public async Task<ContactDto> CreateContactAsync(CreateContactDto dto)
    {
        var contact = new Entities.Contact
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Surname = dto.Surname,
            Company = dto.Company
        };

        var created = await _repository.CreateAsync(contact);

        return new ContactDto
        {
            Id = created.Id,
            Name = created.Name,
            Surname = created.Surname,
            Company = created.Company
        };
    }

    public async Task<bool> DeleteContactAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<ContactInformationDto?> AddContactInformationAsync(Guid contactId, CreateContactInformationDto dto)
    {
        var contactExists = await _repository.ContactExistsAsync(contactId);
        if (!contactExists)
            return null;

        var contactInfo = new ContactInformation
        {
            Id = Guid.NewGuid(),
            ContactId = contactId,
            InfoType = dto.InfoType,
            InfoContent = dto.InfoContent
        };

        var created = await _repository.AddContactInformationAsync(contactInfo);

        return new ContactInformationDto
        {
            Id = created.Id,
            InfoType = created.InfoType,
            InfoContent = created.InfoContent
        };
    }

    public async Task<bool> RemoveContactInformationAsync(Guid contactInformationId)
    {
        return await _repository.RemoveContactInformationAsync(contactInformationId);
    }

    public async Task<LocationStatisticsDto> GetLocationStatisticsAsync(string location)
    {
        var locationInfos = await _repository.GetContactInformationsByLocationAsync(location);
        var contactIds = locationInfos.Select(ci => ci.ContactId).Distinct().ToList();

        var phoneCount = 0;
        foreach (var contactId in contactIds)
        {
            var contact = await _repository.GetByIdWithDetailsAsync(contactId);
            if (contact != null)
                phoneCount += contact.ContactInformations.Count(ci => ci.InfoType == ContactInfoType.PhoneNumber);
        }

        return new LocationStatisticsDto
        {
            Location = location,
            ContactCount = contactIds.Count,
            PhoneNumberCount = phoneCount
        };
    }
}
