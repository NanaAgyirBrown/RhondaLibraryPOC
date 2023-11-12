
using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Common.Errors;

namespace RhondaLibraryPOC.Infrastructure.Common;

internal static class ErrorStatus
{
    internal static ErrorOr<BookDTO?> CheckBookExists(int statusCheck)
    {
        switch (statusCheck)
        {
            case 201:
                return BookErrors.BookFoundRatedX;
            case 202:
                return BookErrors.BookFound;
            case 203:
                return BookErrors.BookFoundInactive;
            case 401:
                return BookErrors.BookFoundSuspended;
            case 404:
                return BookErrors.BookNotFound;
            case 409:
                return BookErrors.BookExits;
            case 400:
                return BookErrors.BookBadSearch;
            case 500:
                return Error.Failure(description: "Db Search ended with an exception");
            default:
                return Error.Failure(description: "unsupported operation");
        }
    }

    internal static ErrorOr<UserDTO?> UserSearchChecker(int statusCheck)
    {
        switch (statusCheck)
        {
            case 201:
                return UserErrors.UserFoundUnconfirmed;
            case 202:
                return UserErrors.UserFound;
            case 203:
                return UserErrors.UserFoundInactive;
            case 401:
                return UserErrors.UserFoundSuspended;
            case 404:
                return UserErrors.UserNotFound;
            case 409:
                return UserErrors.UserExits;
            case 400:
                return UserErrors.UserBadSearch;
            case 500:
                return Error.Failure(description: "Db Search ended with an exception");
            default:
                return Error.Failure(description: "unsupported operation");
        }
    }
}
