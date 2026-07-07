using AppContracts.DeliveryRequests.V1;

namespace AppContracts.DeliveryRequests.V1.Requests;

public class GetRequestsRequestDto
{
    public RequestStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
