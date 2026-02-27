using System.ComponentModel.DataAnnotations;
using PhoneBook.Shared.Enums;

namespace Contact.API.DTOs;

public class CreateContactInformationDto
{
    [Required]
    public ContactInfoType InfoType { get; set; }

    [Required]
    [MaxLength(500)]
    public string InfoContent { get; set; } = string.Empty;
}
