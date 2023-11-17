using Dapper;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Domain.Entity;
using System.Data;
using System.Text.Json;

namespace RhondaLibraryPOC.Infrastructure.Common;

internal static class ParamPrep
{
    internal static DynamicParameters PrepBookParams(Book book)
    {
        DynamicParameters paramlist = new();
        paramlist.Add("vTitle", book.Title, DbType.String, ParameterDirection.Input);
        paramlist.Add("vAuthor", book.Author, DbType.String, ParameterDirection.Input);
        paramlist.Add("vISBN", book.ISBN, DbType.String, ParameterDirection.Input);
        paramlist.Add("vPublisher", book.Publisher, DbType.String, ParameterDirection.Input);
        paramlist.Add("vGenre", book.Genre, DbType.String, ParameterDirection.Input);
        paramlist.Add("vIsAvailable", book.IsAvailable, DbType.Boolean, ParameterDirection.Input);

        return paramlist;
    }

    internal static DynamicParameters PrepUserParams(User user)
    {
        DynamicParameters paramlist = new();
        paramlist.Add("vUsername", user.Username, DbType.String, ParameterDirection.Input);
        paramlist.Add("vFullName", user.FullName, DbType.String, ParameterDirection.Input);
        paramlist.Add("vEmail", user.Email, DbType.String, ParameterDirection.Input);
        paramlist.Add("vAddress", user.Address, DbType.String, ParameterDirection.Input);
        paramlist.Add("vRegisterDate", user.RegistrationDate?.ToUniversalTime(), DbType.DateTime, ParameterDirection.Input);
        paramlist.Add("vId", user.Id, DbType.String, ParameterDirection.Input);
        return paramlist;
    }

    internal static DynamicParameters PrepCheckoutParams((string User, string BookId, string CheckoutId, IEnumerable<CheckoutBook> BookList, DateTime CheckoutDate, DateTime ReturnedDate, bool Returned, decimal Fine) checkout)
    {
        DynamicParameters paramlist = new();
        paramlist.Add("vUser", checkout.User, DbType.String, ParameterDirection.Input);
        paramlist.Add("vBookId", checkout.BookId, DbType.String, ParameterDirection.Input);
        paramlist.Add("vBookList", JsonSerializer.Serialize(checkout.BookList), DbType.String, ParameterDirection.Input);
        paramlist.Add("vCheckoutId", checkout.CheckoutId, DbType.String, ParameterDirection.Input);
        paramlist.Add("vCheckoutDate", checkout.CheckoutDate.ToUniversalTime(), DbType.DateTime, ParameterDirection.Input);
        paramlist.Add("vReturnedDate", checkout.ReturnedDate.ToUniversalTime(), DbType.DateTime, ParameterDirection.Input);
        paramlist.Add("vCheckoutId", checkout.CheckoutId, DbType.String, ParameterDirection.Input);
        paramlist.Add("vReturned", checkout.Returned, DbType.Boolean, ParameterDirection.Input);
        paramlist.Add("vFine", checkout.Fine, DbType.Decimal, ParameterDirection.Input);

        return paramlist;
    }
}
