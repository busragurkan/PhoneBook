namespace PhoneBook.Shared.Events;

public class ReportRequestedEvent
{
    public Guid ReportId { get; set; }
    public string RequestedLocation { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}
