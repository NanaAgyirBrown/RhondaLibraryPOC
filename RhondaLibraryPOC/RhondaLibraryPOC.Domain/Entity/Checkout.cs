using RhondaLibraryPOC.Domain.Common;

namespace RhondaLibraryPOC.Domain.Entity;

public class Checkout : BaseAuditableEntity
{
    public Guid CheckoutId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
    public decimal? Fine { get; set; }
}
