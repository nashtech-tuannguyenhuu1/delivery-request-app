using Core.Domain;

namespace Report.Application.Entities;

public class RequestStatusTracking : IEntity<Guid>
{
    public Guid Id { get; set; }

    public Guid RequestId { get; set; }

    public int Status { get; set; }
}
