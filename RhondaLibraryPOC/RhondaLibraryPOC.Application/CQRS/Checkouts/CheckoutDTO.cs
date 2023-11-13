
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts;

public class CheckoutBookList
{
    public string? Userid { get; set; }
    public IEnumerable<CheckoutBook>? BookList { get; set; }
    public DateTime CheckoutDate { get; set; }
}

public class CheckoutDTO
{
    public string? Userid { get; set; }
    public User? User { get; set; }
    public string? BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public bool Returned { get; set; } = false;
    public decimal Fine { get; set; }
}

public class CheckoutBook
{
    public string? BookId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public bool Returned { get; set; } = false;
    public decimal Fine { get; set; }
}

public record CheckoutRecord
{
    public string? CheckoutId { get; set; }
    public UserRecord User { get; set; }
}