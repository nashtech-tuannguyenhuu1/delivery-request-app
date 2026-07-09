using DeliveryRequest.UI.Models.Audits;
using DeliveryRequest.UI.Models.DeliveryRequests;
using System.Text.Json;

namespace DeliveryRequest.UI.Services;

/// <summary>Typed HttpClient for the Audit API. Failures are normalized into an error result.</summary>
public class AuditApiClient : IAuditApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditApiClient> _logger;

    public AuditApiClient(HttpClient httpClient, ILogger<AuditApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResultDto<List<AuditRecordDto>>> GetAuditsAsync(string primaryKey, string? tableName = null, CancellationToken cancellationToken = default)
    {
        var query = $"v1/audits?primaryKey={Uri.EscapeDataString(primaryKey)}";
        if (!string.IsNullOrEmpty(tableName))
        {
            query += $"&tableName={Uri.EscapeDataString(tableName)}";
        }

        try
        {
            using var response = await _httpClient.GetAsync(query, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Error($"The API request failed with status code {(int)response.StatusCode}.");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<List<AuditRecordDto>>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                return Error(result?.ErrorMessage ?? "No data returned from the API.");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the Audit API.");
            return Error("Could not reach the Audit API. Please try again later.");
        }
    }

    private static ApiResultDto<List<AuditRecordDto>> Error(string message) => new()
    {
        IsError = true,
        ErrorMessage = message,
    };
}
