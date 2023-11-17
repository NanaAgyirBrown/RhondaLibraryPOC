using RhondaLibraryPOC.Domain.Entity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts;

public class CheckoutBookList
{
    public string Userid { get; set; } = null!;
    public IEnumerable<CheckoutBook> BookList { get; set; } = null!;
    public DateTime CheckoutDate { get; set; } = DateTime.UtcNow.ToUniversalTime();
}

public class CheckoutDTO
{
    public string? Userid { get; set; }
    public User? User { get; set; }
    public string? BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime CheckoutDate { get; set; } = DateTime.UtcNow.ToUniversalTime();
    public DateTime? ExpectedReturnDate { get; set; }
    public bool Returned { get; set; } = false;
    public decimal Fine { get; set; }
}

public class CheckoutBook
{
    public string BookId { get; set; } = null!;
    public DateTime CheckoutDate { get; set; } = DateTime.UtcNow.ToUniversalTime();
    public DateTime ExpectedReturnDate { get; set; }
    public bool Returned { get; set; } = false;
    public decimal Fine { get; set; }
}

public class CheckoutRecord
{
    [JsonPropertyName("CheckoutId")]
    public string CheckoutId { get; set; }
    [JsonPropertyName("User")]
    public UserDetail User { get; set; }
}

public record UserCheckout
{
    [JsonPropertyName("User")]
    public UserDetail User { get; init; }

    [JsonPropertyName("Checkouts")]
    public CheckoutDetail Checkouts { get; init; }

    public static UserCheckout FromJson(string json)
    {
        return JsonSerializer.Deserialize<UserCheckout>(json);
    }
}

public record UserDetail
{
    [JsonPropertyName("Email")]
    public string Email { get; init; }

    [JsonPropertyName("UserID")]
    public string UserID { get; init; }

    [JsonPropertyName("FullName")]
    public string FullName { get; init; }
}

public record BookDetail
{
    [JsonPropertyName("Fine")]
    public decimal Fine { get; init; }

    [JsonPropertyName("BookId")]
    public string BookId { get; init; }

    [JsonPropertyName("Title")]
    public string Title { get; init; }

    [JsonPropertyName("Returned")]
    public bool Returned { get; init; }

    [JsonPropertyName("CheckoutDate")]
    public DateTime CheckoutDate { get; init; }

    [JsonPropertyName("ExpectedReturnDate")]
    public DateTime ExpectedReturnDate { get; init; }
}

public record CheckoutDetail
{
    [JsonPropertyName("Books")]
    public List<BookDetail> Books { get; init; }

    [JsonPropertyName("CheckoutId")]
    public string CheckoutId { get; init; }
}