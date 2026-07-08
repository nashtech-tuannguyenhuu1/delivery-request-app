namespace AppContracts.DeliveryRequests.V1.Responses;

public class DocumentResponseDto
{
    public Guid Id { get; set; }

    public Guid RequestId { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public long SizeBytes { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}
