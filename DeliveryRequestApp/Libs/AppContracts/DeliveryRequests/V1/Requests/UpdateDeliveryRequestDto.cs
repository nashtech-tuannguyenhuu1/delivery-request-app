namespace AppContracts.DeliveryRequests.V1.Requests;

public class UpdateDeliveryRequestDto
{
    public required string Title { get; set; }

    public required string PickupAddress { get; set; }

    public required string DeliveryAddress { get; set; }
}
