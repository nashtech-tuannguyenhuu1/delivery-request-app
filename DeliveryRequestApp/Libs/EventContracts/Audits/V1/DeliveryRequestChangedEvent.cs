namespace EventContracts.Audits.V1;

public class DeliveryRequestChangedEvent
{
    public Guid Id { get; set; }

    public int Status { get; set; }
}
