using DeliveryRequest.UI.Models.Reports;
using DeliveryRequest.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryRequest.UI.Controllers;

public class ReportsController : Controller
{
    private readonly IReportApiClient _reportApiClient;

    public ReportsController(IReportApiClient reportApiClient)
    {
        _reportApiClient = reportApiClient;
    }

    public async Task<IActionResult> Index(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var model = new ReportViewModel
        {
            From = from ?? today.AddDays(-6),
            To = to ?? today,
        };

        if (model.From > model.To)
        {
            model.ErrorMessage = "'From' date must be on or before 'To' date.";
            return View(model);
        }

        var result = await _reportApiClient.GetReportByDateRangeAsync(model.From, model.To, cancellationToken);
        if (result.IsError || result.Data is null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No data returned from the API.";
            return View(model);
        }

        model.Report = result.Data;
        return View(model);
    }
}
