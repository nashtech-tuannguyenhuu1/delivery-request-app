namespace DeliveryRequest.UI.Models.DeliveryRequests;

/// <summary>Mirrors the DeliveryRequest.Application.Entities.Request shape returned by the API.</summary>
public class RequestDto
{
    public int Id { get; set; }

    public DeliveryStatus StatusId { get; set; }

    public Guid CreatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public Guid? UpdatedById { get; set; }
}
