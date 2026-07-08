using Mediator.Abstractions;
using Report.Application.UseCases.Queries;

namespace Report.Api.Routes.V1;

public static class ReportEndpointsV1
{
    public static void MapReportEndpointsV1(this WebApplication app)
    {
        var group = app.MapGroup("/v1/reports");

        group.MapGet("/", async (DateOnly from, DateOnly to, ISender sender) =>
        {
            return await sender.SendAsync(new GetReportByDateRange.Query(from, to));
        });
    }
}
