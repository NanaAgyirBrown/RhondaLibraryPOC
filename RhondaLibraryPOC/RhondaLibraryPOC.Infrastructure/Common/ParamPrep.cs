
using Dapper;
using RhondaLibraryPOC.Domain.Entity;
using System.Data;

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
}
