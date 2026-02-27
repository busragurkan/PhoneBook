namespace Contact.API.DTOs;

public class ContactDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public List<ContactInformationDto> ContactInformations { get; set; } = new();
}
