namespace DeliveryRequest.UI.Models.DeliveryRequests;

/// <summary>Mirrors DeliveryRequest.Application.Entities.DeliveryStatus on the API side.</summary>
public enum DeliveryStatus
{
    New = 1,
    Assigned = 2,
    Delivered = 3,
    Returned = 4,
}
