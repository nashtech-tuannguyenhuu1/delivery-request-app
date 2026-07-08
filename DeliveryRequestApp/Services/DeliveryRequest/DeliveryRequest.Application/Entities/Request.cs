using Core.Domain;

namespace DeliveryRequest.Application.Entities;

public class Request : IEntity<Guid>, ICreatableByEntity, ICreatableDateEntity, IUpdatableNullDateEntity, IUpdatableNullEntity,
     IDeletableNullDateEntity, IDeletableNullEntity
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

    public DateTimeOffset? DeletedDate { get; set; }

    public Guid? DeletedById { get; set; }
}
