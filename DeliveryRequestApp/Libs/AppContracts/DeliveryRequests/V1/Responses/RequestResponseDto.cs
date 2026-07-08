namespace AppContracts.DeliveryRequests.V1.Responses;

public class RequestResponseDto
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string PickupAddress { get; set; }

    public required string DeliveryAddress { get; set; }

    public DateTimeOffset? DeliveredDate { get; set; }

    public string? ReturnedReason { get; set; }

    public int StatusId { get; set; }

    public Guid CreatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public Guid? UpdatedById { get; set; }
}
