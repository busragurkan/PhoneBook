namespace Contact.API.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
}
