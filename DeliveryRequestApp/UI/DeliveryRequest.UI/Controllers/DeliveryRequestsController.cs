using AppContracts.DeliveryRequests.V1;
using AppContracts.DeliveryRequests.V1.Requests;
using AppContracts.DeliveryRequests.V1.Responses;
using DeliveryRequest.UI.Models.DeliveryRequests;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DeliveryRequest.UI.Controllers;

public class DeliveryRequestsController : Controller
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly IReadOnlyDictionary<RequestStatus, RequestStatus[]> AllowedTransitions = new Dictionary<RequestStatus, RequestStatus[]>
    {
        [RequestStatus.New] = new[] { RequestStatus.Assigned },
        [RequestStatus.Assigned] = new[] { RequestStatus.Delivered, RequestStatus.Returned },
        [RequestStatus.Delivered] = Array.Empty<RequestStatus>(),
        [RequestStatus.Returned] = Array.Empty<RequestStatus>(),
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DeliveryRequestsController> _logger;

    public DeliveryRequestsController(IHttpClientFactory httpClientFactory, ILogger<DeliveryRequestsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index(RequestStatus? status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
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

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<PagedResultDto<RequestResponseDto>>>(JsonOptions, cancellationToken);
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

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            using var response = await client.GetAsync($"v1/requests/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<RequestResponseDto>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                return NotFound();
            }

            var model = BuildDetailsViewModel(result.Data);

            using var documentsResponse = await client.GetAsync($"v1/requests/{id}/documents", cancellationToken);
            if (documentsResponse.IsSuccessStatusCode)
            {
                var documentsResult = await documentsResponse.Content.ReadFromJsonAsync<ApiResultDto<List<DocumentResponseDto>>>(JsonOptions, cancellationToken);
                if (documentsResult is { IsError: false, Data: not null })
                {
                    model.Documents = documentsResult.Data;
                }
            }

            if (TempData["ErrorMessage"] is string tempError)
            {
                model.ErrorMessage = tempError;
            }

            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            return Problem("Could not reach the DeliveryRequest API. Please try again later.");
        }
    }

    public IActionResult Create()
    {
        return View(new DeliveryRequestFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeliveryRequestFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");
        var dto = new CreateDeliveryRequestDto
        {
            Title = model.Title,
            PickupAddress = model.PickupAddress,
            DeliveryAddress = model.DeliveryAddress,
        };

        try
        {
            using var response = await client.PostAsJsonAsync("v1/requests", dto, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = $"The API request failed with status code {(int)response.StatusCode}.";
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<Guid>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError)
            {
                model.ErrorMessage = result?.ErrorMessage ?? "Failed to create the delivery request.";
                return View(model);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            model.ErrorMessage = "Could not reach the DeliveryRequest API. Please try again later.";
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            using var response = await client.GetAsync($"v1/requests/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<RequestResponseDto>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                return NotFound();
            }

            var model = new DeliveryRequestFormViewModel
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                PickupAddress = result.Data.PickupAddress,
                DeliveryAddress = result.Data.DeliveryAddress,
            };

            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            return Problem("Could not reach the DeliveryRequest API. Please try again later.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DeliveryRequestFormViewModel model, CancellationToken cancellationToken)
    {
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");
        var dto = new UpdateDeliveryRequestDto
        {
            Title = model.Title,
            PickupAddress = model.PickupAddress,
            DeliveryAddress = model.DeliveryAddress,
        };

        try
        {
            using var response = await client.PutAsJsonAsync($"v1/requests/{id}", dto, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = $"The API request failed with status code {(int)response.StatusCode}.";
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<bool>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError)
            {
                model.ErrorMessage = result?.ErrorMessage ?? "Failed to update the delivery request.";
                return View(model);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            model.ErrorMessage = "Could not reach the DeliveryRequest API. Please try again later.";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            using var response = await client.GetAsync($"v1/requests/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<RequestResponseDto>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError || result.Data is null)
            {
                return NotFound();
            }

            return View(result.Data);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            return Problem("Could not reach the DeliveryRequest API. Please try again later.");
        }
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            using var response = await client.DeleteAsync($"v1/requests/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"The API request failed with status code {(int)response.StatusCode}.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<bool>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError)
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Failed to delete the delivery request.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            TempData["ErrorMessage"] = "Could not reach the DeliveryRequest API. Please try again later.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(Guid id, RequestStatus status, string? reason, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");
        var dto = new UpdateRequestStatusDto
        {
            Status = status,
            Reason = reason,
        };

        try
        {
            using var response = await client.PatchAsJsonAsync($"v1/requests/{id}/status", dto, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"The API request failed with status code {(int)response.StatusCode}.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<bool>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError)
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Failed to change the status of the delivery request.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            TempData["ErrorMessage"] = "Could not reach the DeliveryRequest API. Please try again later.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private static DeliveryRequestDetailsViewModel BuildDetailsViewModel(RequestResponseDto request)
    {
        var currentStatus = (RequestStatus)request.StatusId;
        var allowedNextStatuses = AllowedTransitions.TryGetValue(currentStatus, out var next) ? next : Array.Empty<RequestStatus>();

        return new DeliveryRequestDetailsViewModel
        {
            Request = request,
            AllowedNextStatuses = allowedNextStatuses,
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please choose a file to upload.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            using var response = await client.PostAsync($"v1/requests/{id}/documents", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"The API request failed with status code {(int)response.StatusCode}.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResultDto<Guid>>(JsonOptions, cancellationToken);
            if (result is null || result.IsError)
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Failed to upload the document.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            TempData["ErrorMessage"] = "Could not reach the DeliveryRequest API. Please try again later.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> DownloadDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("DeliveryRequestApi");

        try
        {
            var response = await client.GetAsync($"v1/documents/{documentId}/download", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName
                ?? "download";

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return File(stream, contentType, fileName.Trim('"'));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call the DeliveryRequest API.");
            return Problem("Could not reach the DeliveryRequest API. Please try again later.");
        }
    }
}
