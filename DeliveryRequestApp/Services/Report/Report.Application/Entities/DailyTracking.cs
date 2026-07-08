using Core.Domain;

namespace Report.Application.Entities;

public class DailyTracking : IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public int NewCount { get; set; }

    public int AssignedCount { get; set; }

    public int DeliveredCount { get; set; }

    public int ReturnedCount { get; set; }
}
