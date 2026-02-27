using System.ComponentModel.DataAnnotations;

namespace Contact.API.DTOs;

public class CreateContactDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;
}
