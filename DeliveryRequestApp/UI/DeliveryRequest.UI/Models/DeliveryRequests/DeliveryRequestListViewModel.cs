namespace DeliveryRequest.UI.Models.DeliveryRequests;

public class DeliveryRequestListViewModel
{
    public IReadOnlyList<RequestDto> Items { get; set; } = Array.Empty<RequestDto>();

    public DeliveryStatus? SelectedStatus { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPrevious { get; set; }

    public bool HasNext { get; set; }

    public string? ErrorMessage { get; set; }
}
