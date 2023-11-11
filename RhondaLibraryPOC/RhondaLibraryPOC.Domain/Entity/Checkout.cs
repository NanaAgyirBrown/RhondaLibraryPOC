using RhondaLibraryPOC.Domain.Common;

namespace RhondaLibraryPOC.Domain.Entity;

public class Checkout : BaseAuditableEntity
{
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public string BookId { get; set; }
    public Book Book { get; set; } = null!;
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
    public decimal? Fine { get; set; }
}
