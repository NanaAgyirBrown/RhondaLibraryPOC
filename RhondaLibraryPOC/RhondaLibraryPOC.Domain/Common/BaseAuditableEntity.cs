namespace RhondaLibraryPOC.Domain.Common;

public class BaseAuditableEntity : IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}