namespace Report.Application.Responses;

public record ReportTotalDto
{
    public DateOnly From { get; init; }

    public DateOnly To { get; init; }

    public int NewCount { get; init; }

    public int AssignedCount { get; init; }

    public int DeliveredCount { get; init; }

    public int ReturnedCount { get; init; }
}
