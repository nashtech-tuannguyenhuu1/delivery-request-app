using Report.Api.Routes.V1;

namespace Report.Api.Routes;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        app.MapReportEndpointsV1();
    }
}
