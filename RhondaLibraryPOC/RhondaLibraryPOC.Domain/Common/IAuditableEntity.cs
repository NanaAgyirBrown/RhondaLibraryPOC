namespace RhondaLibraryPOC.Domain.Common;

internal interface IAuditableEntity : IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
