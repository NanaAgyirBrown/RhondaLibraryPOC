namespace RhondaLibraryPOC.Domain.Common;

public class BaseAuditableEntity : IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}