using Core.Domain;

namespace DeliveryRequest.Application.Entities;

public class Document : IEntity<Guid>, ICreatableByEntity, ICreatableDateEntity
{
    public Guid Id { get; set; }

    public Guid RequestId { get; set; }

    public required string FileName { get; set; }

    public required string ContentType { get; set; }

    public long SizeBytes { get; set; }

    public required string BlobName { get; set; }

    public Guid CreatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}
