namespace RhondaLibraryPOC.Domain.Common;

internal interface IAuditableEntity : IEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
