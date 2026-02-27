using PhoneBook.Shared.Enums;

namespace Contact.API.DTOs;

public class ContactInformationDto
{
    public Guid Id { get; set; }
    public ContactInfoType InfoType { get; set; }
    public string InfoContent { get; set; } = string.Empty;
}
