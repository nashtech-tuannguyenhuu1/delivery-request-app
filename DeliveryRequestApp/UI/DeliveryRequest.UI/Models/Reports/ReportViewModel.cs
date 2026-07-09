using AppContracts.Reports.V1.Responses;

namespace DeliveryRequest.UI.Models.Reports;

public class ReportViewModel
{
    public DateOnly From { get; set; }

    public DateOnly To { get; set; }

    public ReportTotalResponseDto? Report { get; set; }

    public string? ErrorMessage { get; set; }
}
