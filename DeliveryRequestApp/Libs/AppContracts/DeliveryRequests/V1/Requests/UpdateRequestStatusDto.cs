namespace AppContracts.DeliveryRequests.V1.Requests;

public class UpdateRequestStatusDto
{
    public RequestStatus Status { get; set; }

    public string? Reason { get; set; }
}
