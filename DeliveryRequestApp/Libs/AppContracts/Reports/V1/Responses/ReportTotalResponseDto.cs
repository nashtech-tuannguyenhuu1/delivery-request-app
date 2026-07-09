namespace AppContracts.Reports.V1.Responses;

public record ReportTotalResponseDto
{
    public DateOnly From { get; init; }

    public DateOnly To { get; init; }

    public int NewCount { get; init; }

    public int AssignedCount { get; init; }

    public int DeliveredCount { get; init; }

    public int ReturnedCount { get; init; }
}
