namespace DeliveryRequest.UI.Models.Reports;

public class ReportViewModel
{
    public DateOnly From { get; set; }

    public DateOnly To { get; set; }

    public ReportTotalDto? Report { get; set; }

    public string? ErrorMessage { get; set; }
}
