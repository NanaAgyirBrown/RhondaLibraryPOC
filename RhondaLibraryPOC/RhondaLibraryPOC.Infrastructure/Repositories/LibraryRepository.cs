
using Dapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
using RhondaLibraryPOC.Application.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;
using RhondaLibraryPOC.Application.CQRS.Users.Queries;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Interfaces.Persistence;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Common.Errors;
using RhondaLibraryPOC.Domain.Entity;
using RhondaLibraryPOC.Infrastructure.Common;
using System.Data;
using System.Text.Json;

namespace RhondaLibraryPOC.Persistence.Repositories;

public class LibraryRepository : IBookRepository, ICheckoutRepository, IUserRepository
{
    private readonly ILogger<LibraryRepository> _logger;
    private readonly IDataSource _dataSource;

    public LibraryRepository(ILogger<LibraryRepository> logger, IDataSource dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    public async Task<ErrorOr<Book?>> AddBook(Book book)
    {
        _logger.LogInformation("Adding book {@book} at {DateTime}", book, DateTime.Now);

        book.CreatedAt = DateTime.Now;
        book.ModifiedAt = DateTime.Now;

        DbResponse? dbResponse = null;

        try
        {
            _logger.LogInformation("Creating connection at {DateTime}", DateTime.Now);

            using(var connection = await _dataSource.CreateConnection())
            {
                var paramlist = PrepBookParams(book);
                _logger.LogInformation("Checking if Book Exist with ISBN ");
                dbResponse = await connection.QueryFirstAsync<DbResponse?>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookNotFound")
                {
                    _logger.LogInformation("Book does not exist, adding book to database. Creating new record.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Shelve.add_book(@vIsbn, @vTitle, @vAuthor, @vPublisher, @vGenre, @vIsAvailable)", paramlist);

                    if (dbResponse.StatusCode == 200)
                    {
                        _logger.LogInformation("Book added successfully. Returning book details.");
                        var bookDTO = JsonSerializer.Deserialize<Book?>(dbResponse.Details, options: null);

                        return bookDTO;
                    }
                    else
                        return Error.Failure(
                                code: dbResponse.Status,
                                description: dbResponse.Details);

                }
                else
                    return status;
            }
        }
        catch(Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book saving exception occurred : {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);

            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }

    }

    private DynamicParameters PrepBookParams(Book book)
    {
        _logger.LogInformation("Preparing book parameters for {@book} at {DateTime}", book, DateTime.Now);

        DynamicParameters paramlist = new();
        paramlist.Add("vTitle", book.Title, DbType.String, ParameterDirection.Input);
        paramlist.Add("vAuthor", book.Author, DbType.String, ParameterDirection.Input);
        paramlist.Add("vISBN", book.ISBN, DbType.String, ParameterDirection.Input);
        paramlist.Add("vPublisher", book.Publisher, DbType.String, ParameterDirection.Input);
        paramlist.Add("vGenre", book.Genre, DbType.String, ParameterDirection.Input);
        paramlist.Add("vIsAvailable", book.IsAvailable, DbType.Boolean, ParameterDirection.Input);

        return paramlist;
    }

    private static ErrorOr<Book?> CheckBookExists(int statusCheck)
    {
        switch (statusCheck)
        {
            case 201:
                return BookErrors.BookFoundUnconfirmed;
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

    public Task<UserDTO> AddUser(AddUserCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<CheckoutDTO> BooksNotReturned(GetBooksNotReturnedQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<CheckoutDTO> CheckoutBooks(CheckoutBooksCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<BookDTO> DeleteBook(DeleteBookCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<BookDTO> GetBookById(GetBookDetailsQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BookDTO>> GetBooks(GetAllBooksQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> GetUserById(GetUserDetailsQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<CheckoutDTO> ReturnBooks(ReturnBooksCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<BookDTO> UpdateBook(UpdateBookCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> UpdateUser(UpdateUserCommand command)
    {
        throw new NotImplementedException();
    }
}
