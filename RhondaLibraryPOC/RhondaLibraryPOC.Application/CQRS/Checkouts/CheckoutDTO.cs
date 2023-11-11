
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts;

public class CheckoutDTO
{
    public string Id { get; set; }
    public string? Userid { get; set; }
    public User? User { get; set; }
    public string? BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime ExpectedReturnDate {  get; set; }
    public bool Returned { get; set; } = false;
    public decimal Fine {  get; set; }
}
