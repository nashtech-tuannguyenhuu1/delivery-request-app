using System.Text.Json;
using DeliveryRequest.UI.Models.DeliveryRequests;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryRequest.UI.Controllers;

public class DeliveryRequestsController : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DeliveryRequestsController> _logger;

    public DeliveryRequestsController(IHttpClientFactory httpClientFactory, ILogger<DeliveryRequestsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DeliveryStatus? status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var model = new DeliveryRequestListViewModel
        {
            SelectedStatus = status,
            PageNumber = page,
            PageSize = pageSize,
        };

        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");
        var query = $"v1/requests?page={page}&pageSize={pageSize}" + (status.HasValue ? $"&status={status}" : string.Empty);

        try
        {
            using var response = await client.GetAsync(query, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = $"The API request failed with status code {(int)response.StatusCode}.";
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<PagedResultDto<RequestDto>>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                model.ErrorMessage = result?.ErrorMessage ?? "No data returned from the API.";
                return View(model);
            }

            model.Items = result.Data.Items;
            model.TotalCount = result.Data.TotalCount;
            model.TotalPages = result.Data.TotalPages;
            model.HasPrevious = result.Data.HasPrevious;
            model.HasNext = result.Data.HasNext;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            model.ErrorMessage = "Could not reach the DeliveryRequest API. Please try again later.";
        }

        return View(model);
    }
}
