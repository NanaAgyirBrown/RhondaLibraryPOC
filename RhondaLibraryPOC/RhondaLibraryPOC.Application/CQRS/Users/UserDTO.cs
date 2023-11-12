
using RhondaLibraryPOC.Application.CQRS.Common;
using System.ComponentModel;

namespace RhondaLibraryPOC.Application.Users;

public class UserDTO : Identifier
{    
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime? RegistrationDate { get; set; }

}

public record UserRecord(
    string UserID,
    string Username,
    string FullName,
    string Email,
    string Address,
    DateTime RegisteredOn,
    IEnumerable<UserCheckout> Checkouts
);

public record UserCheckout(
    string ISBN,
    string Title,
    DateTime CheckoutDate,
    DateTime ReturnDate,
    bool Returned,
    decimal Fine
);