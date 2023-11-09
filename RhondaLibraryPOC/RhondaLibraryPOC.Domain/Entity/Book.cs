using RhondaLibraryPOC.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RhondaLibraryPOC.Domain.Entity;

public class Book : BaseAuditableEntity
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    [Required]
    public string? ISBN { get; set; }
    public string? Publisher { get; set; }
    public string? Genre { get; set; }
    public bool IsAvailable { get; set; }
}
