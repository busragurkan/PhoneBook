using Contact.API.Entities;

namespace Contact.API.Repositories;

public interface IContactRepository
{
    Task<List<Entities.Contact>> GetAllAsync();
    Task<Entities.Contact?> GetByIdAsync(Guid id);
    Task<Entities.Contact?> GetByIdWithDetailsAsync(Guid id);
    Task<Entities.Contact> CreateAsync(Entities.Contact contact);
    Task<bool> DeleteAsync(Guid id);
    Task<ContactInformation> AddContactInformationAsync(ContactInformation contactInformation);
    Task<bool> RemoveContactInformationAsync(Guid contactInformationId);
    Task<bool> ContactExistsAsync(Guid contactId);
    Task<List<ContactInformation>> GetContactInformationsByLocationAsync(string location);
}
