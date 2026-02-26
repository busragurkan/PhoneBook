namespace PhoneBook.Shared.DTOs;

public class LocationStatisticsDto
{
    public string Location { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public int PhoneNumberCount { get; set; }
}
