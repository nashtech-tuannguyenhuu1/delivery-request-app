using AppContracts.Reports.V1.Responses;
using DeliveryRequest.UI.Models.DeliveryRequests;
using System.Text.Json;

namespace DeliveryRequest.UI.Services;

/// <summary>Typed HttpClient for the Report API. Failures are normalized into an error result.</summary>
public class ReportApiClient : IReportApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<ReportApiClient> _logger;

    public ReportApiClient(HttpClient httpClient, ILogger<ReportApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResultDto<ReportTotalResponseDto>> GetReportByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        var query = $"v1/reports?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

        try
        {
            using var response = await _httpClient.GetAsync(query, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Error($"The API request failed with status code {(int)response.StatusCode}.");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<ReportTotalResponseDto>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                return Error(result?.ErrorMessage ?? "No data returned from the API.");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the Report API.");
            return Error("Could not reach the Report API. Please try again later.");
        }
    }

    private static ApiResultDto<ReportTotalResponseDto> Error(string message) => new()
    {
        IsError = true,
        ErrorMessage = message,
    };
}
