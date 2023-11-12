using ErrorOr;

namespace RhondaLibraryPOC.Domain.Common.Errors;

public static class BookErrors
{
    public static Error BookExits => Error.Validation(
        code: "Book.BookExists",
        description: "Book creation failed. Book with ISBN already exists."
    );

    public static Error InvalidBooknameFormat => Error.Validation(
            code: "Book.InvalidBooknameFormat",
            description: "Invalid provided Book details. Please provide valid book details."
    );

    public static Error BookNotFound => Error.NotFound(
            code: "Book.BookNotFound",
            description: "Book not found."
    );

    public static Error BookFoundInactive => Error.Validation(
            code: "Book.BookFoundInactive",
            description: "Book currently unavailable for lending."
    );

    public static Error BookFoundSuspended => Error.Validation(
           code: "Book.BookFoundSuspended",
           description: "Book has been marked suspended from category. Contact Librarian."
   );

    public static Error BookFound => Error.Custom(
            type: 200,
            code: "Book.BookFound",
            description: "Book details found."
    );

    public static Error BookBadSearch => Error.Unexpected(
           code: "Book.BookBadSearch",
           description: "Could not determine Book exists or not."
    );

    public static Error BookFoundRatedX => Error.Validation(
       code: "Book.BookFoundRatedX",
       description: "CAUTION!!!! This Book can not be lend to Users under 18 years. Please make sure User is over 18 years"
    );
}

public static class UserErrors
{
    public static Error UserExits => Error.Validation(
        code: "User.UserExists",
        description: "User creation failed. User already exists."
    );

    public static Error InvalidUserFormat => Error.Validation(
            code: "User.InvalidUserFormat",
            description: "User details provided not valid. Please provide valid details."
    );

    public static Error UserNotFound => Error.NotFound(
            code: "User.UserNotFound",
            description: "User details not found."
    );

    public static Error UserFoundInactive => Error.Validation(
            code: "User.UserFoundInactive",
            description: "User account currently inactive."
    );

    public static Error UserFoundSuspended => Error.Validation(
           code: "User.UserFoundSuspended",
           description: "User account has been marked suspended for some violations. Contact Librarian."
   );

    public static Error UserFound => Error.Validation(
            code: "User.UserFound",
            description: "User account details found."
    );

    public static Error UserBadSearch => Error.Unexpected(
           code: "User.UserBadSearch",
           description: "Could not determine User account exists or not."
    );

    public static Error UserFoundUnconfirmed => Error.Validation(
       code: "User.UserFoundConfirmed",
       description: "User account requires some admin action."
    );
}