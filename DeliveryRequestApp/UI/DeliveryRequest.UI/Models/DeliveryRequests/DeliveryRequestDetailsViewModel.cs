using AppContracts.DeliveryRequests.V1;
using AppContracts.DeliveryRequests.V1.Responses;

namespace DeliveryRequest.UI.Models.DeliveryRequests;

public class DeliveryRequestDetailsViewModel
{
    public required RequestResponseDto Request { get; set; }

    public IReadOnlyList<RequestStatus> AllowedNextStatuses { get; set; } = Array.Empty<RequestStatus>();

    public string? ErrorMessage { get; set; }

    public string? StatusMessage { get; set; }
}
