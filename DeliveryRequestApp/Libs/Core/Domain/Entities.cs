namespace Core.Domain;

public interface IEntity
{
}

public interface IEntity<T> : IEntity
{
    T Id { get; set; }
}


public interface ICreatableByEntity
{
    public Guid CreatedById { get; set; }
}

public interface ICreatableDateEntity
{
    DateTimeOffset CreatedDate { get; set; }
}


public interface IUpdatableNullEntity
{
    Guid? UpdatedById { get; set; }
}

public interface IUpdatableNullDateEntity
{
    DateTimeOffset? UpdatedDate { get; set; }
}
