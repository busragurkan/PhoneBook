namespace Contact.API.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;

    public ICollection<ContactInformation> ContactInformations { get; set; } = new List<ContactInformation>();
}
