using PhoneBook.Shared.Enums;

namespace Contact.API.Entities;

public class ContactInformation
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public ContactInfoType InfoType { get; set; }
    public string InfoContent { get; set; } = string.Empty;

    public Contact Contact { get; set; } = null!;
}
