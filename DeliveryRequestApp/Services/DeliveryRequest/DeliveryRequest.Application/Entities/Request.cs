using Core.Domain;

namespace DeliveryRequest.Application.Entities;

public class Request : IEntity<int>, ICreatableByEntity, ICreatableDateEntity, IUpdatableNullDateEntity, IUpdatableNullEntity
{
    public int Id { get; set; }

    public int StatusId { get; set; }

    public Guid CreatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public Guid? UpdatedById { get; set; }
}
