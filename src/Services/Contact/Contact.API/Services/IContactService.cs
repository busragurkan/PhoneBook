using Contact.API.DTOs;
using PhoneBook.Shared.DTOs;

namespace Contact.API.Services;

public interface IContactService
{
    Task<List<ContactDto>> GetAllContactsAsync();
    Task<ContactDetailDto?> GetContactDetailAsync(Guid id);
    Task<ContactDto> CreateContactAsync(CreateContactDto dto);
    Task<bool> DeleteContactAsync(Guid id);
    Task<ContactInformationDto?> AddContactInformationAsync(Guid contactId, CreateContactInformationDto dto);
    Task<bool> RemoveContactInformationAsync(Guid contactInformationId);
    Task<LocationStatisticsDto> GetLocationStatisticsAsync(string location);
}
