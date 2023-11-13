using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Common.Errors;

namespace RhondaLibraryPOC.Infrastructure.Common;

internal static class ErrorStatus
{
    internal static ErrorOr<BookDTO?> CheckBookExists(int statusCheck)
    {
        return statusCheck switch
        {
            201 => (ErrorOr<BookDTO?>)BookErrors.BookFoundRatedX,
            202 => (ErrorOr<BookDTO?>)BookErrors.BookFound,
            203 => (ErrorOr<BookDTO?>)BookErrors.BookFoundInactive,
            401 => (ErrorOr<BookDTO?>)BookErrors.BookFoundSuspended,
            404 => (ErrorOr<BookDTO?>)BookErrors.BookNotFound,
            409 => (ErrorOr<BookDTO?>)BookErrors.BookExits,
            400 => (ErrorOr<BookDTO?>)BookErrors.BookBadSearch,
            500 => (ErrorOr<BookDTO?>)Error.Failure(description: "Db Search ended with an exception"),
            _ => (ErrorOr<BookDTO?>)Error.Failure(description: "unsupported operation"),
        };
    }

    internal static ErrorOr<UserDTO?> UserSearchChecker(int statusCheck)
    {
        return statusCheck switch
        {
            201 => (ErrorOr<UserDTO?>)UserErrors.UserFoundUnconfirmed,
            202 => (ErrorOr<UserDTO?>)UserErrors.UserFound,
            203 => (ErrorOr<UserDTO?>)UserErrors.UserFoundInactive,
            401 => (ErrorOr<UserDTO?>)UserErrors.UserFoundSuspended,
            404 => (ErrorOr<UserDTO?>)UserErrors.UserNotFound,
            409 => (ErrorOr<UserDTO?>)UserErrors.UserExits,
            400 => (ErrorOr<UserDTO?>)UserErrors.UserBadSearch,
            500 => (ErrorOr<UserDTO?>)Error.Failure(description: "Db Search ended with an exception"),
            _ => (ErrorOr<UserDTO?>)Error.Failure(description: "unsupported operation"),
        };
    }
}
