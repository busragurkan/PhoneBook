namespace Report.API.Entities;

public enum ReportStatus
{
    Preparing = 0,
    Completed = 1
}

public class Report
{
    public Guid Id { get; set; }
    public string RequestedLocation { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public int ContactCount { get; set; }
    public int PhoneNumberCount { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
