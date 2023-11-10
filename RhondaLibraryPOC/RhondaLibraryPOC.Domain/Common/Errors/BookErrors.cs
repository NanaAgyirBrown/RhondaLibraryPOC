using ErrorOr;

namespace RhondaLibraryPOC.Domain.Common.Errors;

public static class BookErrors
{
    public static Error BookExits => Error.Failure(
        code: "Book.BookExists",
        description: "Sign up failed. Bookname already exists."
    );

    public static Error InvalidBooknameFormat => Error.Validation(
            code: "Book.InvalidBooknameFormat",
            description: "Invalid providered Bookname. Please provide a valid email address or phonenumber."
    );

    public static Error BookNotFound => Error.NotFound(
            code: "Book.BookNotFound",
            description: "Book not found as a signed up Book."
    );

    public static Error BookFoundInactive => Error.Validation(
            code: "Book.BookFoundInactive",
            description: "Book account is currently inactive."
    );

    public static Error BookFoundSuspended => Error.Validation(
           code: "Book.BookFoundSuspended",
           description: "Book account has been suspended. Contact customer care."
   );

    public static Error BookFoundUnconfirmed => Error.Validation(
           code: "Book.BookFoundUnconfirmed",
           description: "Book account sign up has not been confirmed. Please confirm to activate your account."
   );

    public static Error BookFound => Error.Validation(
            code: "Book.BookFound",
            description: "Book account found."
    );

    public static Error BookBadSearch => Error.Failure(
           code: "Book.BookBadSearch",
           description: "Could not determine Book exists or not."
    );

}
