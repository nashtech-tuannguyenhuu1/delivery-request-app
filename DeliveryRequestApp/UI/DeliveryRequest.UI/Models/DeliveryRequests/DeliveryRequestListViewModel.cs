using AppContracts.DeliveryRequests.V1;
using AppContracts.DeliveryRequests.V1.Responses;

namespace DeliveryRequest.UI.Models.DeliveryRequests;

public class DeliveryRequestListViewModel
{
    public IReadOnlyList<RequestResponseDto> Items { get; set; } = Array.Empty<RequestResponseDto>();

    public RequestStatus? SelectedStatus { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPrevious { get; set; }

    public bool HasNext { get; set; }

    public string? ErrorMessage { get; set; }
}
