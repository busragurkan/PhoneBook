using Contact.API.Data;
using Contact.API.Entities;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Shared.Enums;

namespace Contact.API.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ContactDbContext _context;

    public ContactRepository(ContactDbContext context)
    {
        _context = context;
    }

    public async Task<List<Entities.Contact>> GetAllAsync()
    {
        return await _context.Contacts
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Surname)
            .ToListAsync();
    }

    public async Task<Entities.Contact?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Entities.Contact?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Contacts
            .Include(c => c.ContactInformations)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Entities.Contact> CreateAsync(Entities.Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
            return false;

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ContactInformation> AddContactInformationAsync(ContactInformation contactInformation)
    {
        _context.ContactInformations.Add(contactInformation);
        await _context.SaveChangesAsync();
        return contactInformation;
    }

    public async Task<bool> RemoveContactInformationAsync(Guid contactInformationId)
    {
        var info = await _context.ContactInformations.FindAsync(contactInformationId);
        if (info == null)
            return false;

        _context.ContactInformations.Remove(info);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ContactExistsAsync(Guid contactId)
    {
        return await _context.Contacts.AnyAsync(c => c.Id == contactId);
    }

    public async Task<List<ContactInformation>> GetContactInformationsByLocationAsync(string location)
    {
        return await _context.ContactInformations
            .Include(ci => ci.Contact)
            .Where(ci => ci.InfoType == ContactInfoType.Location && ci.InfoContent == location)
            .AsNoTracking()
            .ToListAsync();
    }
}
