using System.ComponentModel.DataAnnotations;

namespace Report.API.DTOs;

public class CreateReportRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;
}
