
using Dapper;
using ErrorOr;
using Microsoft.Extensions.Logging;
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
using RhondaLibraryPOC.Domain.Entity;
using RhondaLibraryPOC.Infrastructure.Common;
using System.Text.Json;

namespace RhondaLibraryPOC.Persistence.Repositories;

public class LibraryRepository : IBookRepository, ICheckoutRepository, IUserRepository, IExtrasRepository
{
    private readonly ILogger<LibraryRepository> _logger;
    private readonly IDataSource _dataSource;

    public LibraryRepository(ILogger<LibraryRepository> logger, IDataSource dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    public async Task<ErrorOr<BookDTO>> AddBook(Book book, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding book {book} at {DateTime}", book, DateTime.Now);

            book.CreatedOn = DateTime.Now;
            book.LastUpdatedOn = DateTime.Now;

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing book parameters for {Book} at {DateTime}", book, DateTime.Now);
                var paramlist = ParamPrep.PrepBookParams(book);

                _logger.LogInformation("Checking if Book Exist with ISBN ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookNotFound")
                {
                    _logger.LogInformation("Book does not exist, adding book to the database. Creating a new record.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Shelve.add_book(@vIsbn, @vTitle, @vAuthor, @vPublisher, @vGenre, @vIsAvailable)", paramlist);

                    if (dbResponse.StatusCode == (int)EnumStatus.Success)
                    {
                        _logger.LogInformation("Book added successfully. Returning book details.");
                        return JsonSerializer.Deserialize<BookDTO>(dbResponse.Details, options: null);
                    }
                    else
                    {
                        return Error.Failure(
                                code: dbResponse.Status,
                                description: dbResponse.Details);
                    }
                }
                else
                {
                    return status;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book saving exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<IEnumerable<BookDTO>>> GetBooks(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting a list of books at {DateTime}", DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Getting all books from the database at {DateTime}", DateTime.Now);
                var response = await connection.QuerySingleAsync<DbResponse>("Select * from shelve.list_books()");

                if (response.StatusCode == (int)EnumStatus.Success)
                {
                    _logger.LogInformation("Books retrieved successfully. Returning book details.");
                    return JsonSerializer.Deserialize<IEnumerable<BookDTO>>(response.Details, options: null).ToList();
                }
                else
                {
                    return Error.Failure(
                          code: response.Status,
                          description: response.Details);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - retrieving a list of Books encountered an exception: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<BookDTO>> GetBookById(GetBookDetailsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting book details for ISBN {Isbn} at {DateTime}", query.Isbn, DateTime.Now);

            var book = new Book
            {
                ISBN = query.Isbn
            };

            var paramlist = ParamPrep.PrepBookParams(book);

            using (var connection = await _dataSource.CreateConnection())
            {
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);
                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookNotFound")
                {
                    _logger.LogInformation("Book does not exist, returning null details.");
                    return Error.NotFound(
                            code: dbResponse.StatusCode.ToString(),
                            description: dbResponse.Details);
                }

                _logger.LogInformation("Getting book details from the database at {DateTime}", DateTime.Now);
                var response = await connection.QuerySingleAsync<DbResponse>("Select * from shelve.get_book(@vIsbn)", paramlist);

                if (response.StatusCode == (int)EnumStatus.Success)
                {
                    _logger.LogInformation("Book details retrieved successfully. Returning book details.");
                    return JsonSerializer.Deserialize<BookDTO>(response.Details, options: null);
                }
                else
                {
                    return Error.Failure(
                          code: response.Status,
                          description: response.Details);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - Getting book details encountered an exception: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<BookDTO>> UpdateBook(UpdateBookCommand command, CancellationToken cancellation)
    {
        try
        {
            _logger.LogInformation("Updating book {book} at {DateTime}", command.Isbn, DateTime.Now);

            var book = new Book
            {
                ISBN = command.Isbn,
                Title = command.Title,
                Author = command.Author,
                Publisher = command.Publisher,
                Genre = command.Genre,
                IsAvailable = command.IsAvailable ?? false
            };

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing book parameters for {Book} at {DateTime}", book, DateTime.Now);
                var paramlist = ParamPrep.PrepBookParams(book);

                _logger.LogInformation("Checking if Book Exist with ISBN ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookNotFound")
                {
                    _logger.LogInformation("Book does not exist, can not update book details.");

                    return Error.NotFound(
                            code: dbResponse.StatusCode.ToString(),
                            description: dbResponse.Details);
                }
                else
                {
                    _logger.LogInformation("Updating book details in database.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Shelve.update_book(@vIsbn, @vTitle, @vAuthor, @vPublisher, @vGenre, @vIsAvailable)", paramlist);

                    if (dbResponse.StatusCode == (int)EnumStatus.Success)
                    {
                        _logger.LogInformation("Book details successfully updated. Returning book details.");
                        return JsonSerializer.Deserialize<BookDTO>(dbResponse.Details, options: null);
                    }

                    return Error.Failure(
                            code: dbResponse.Status,
                            description: dbResponse.Details);

                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book saving exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<BookDTO>> DeleteBook(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting book - {book} at {DateTime}", command.Isbn, DateTime.Now);

            var book = new Book
            {
                ISBN = command.Isbn
            };

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing book parameters for {Book} at {DateTime}", book, DateTime.Now);
                var paramlist = ParamPrep.PrepBookParams(book);

                _logger.LogInformation("Checking if Book Exist with ISBN ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookNotFound")
                {
                    _logger.LogInformation("Book does not exist, can not delete book details.");

                    return Error.NotFound(
                            code: dbResponse.StatusCode.ToString(),
                            description: dbResponse.Details);
                }

                _logger.LogInformation("Deleting book details from database.");
                dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Shelve.remove_book(@vIsbn)", paramlist);

                if (dbResponse.StatusCode == (int)EnumStatus.Success)
                {
                    _logger.LogInformation("Book details successfully updated. Returning book details.");
                    return JsonSerializer.Deserialize<BookDTO>(dbResponse.Details, options: null);
                }

                return Error.Failure(
                        code: dbResponse.Status,
                        description: dbResponse.Details);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book saving exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<UserDTO>> AddUser(User command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new User {User} at {DateTime}", command, DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Creating new User {User} at {DateTime}", command, DateTime.Now);
                var paramlist = ParamPrep.PrepUserParams(command);

                _logger.LogInformation("Checking if User Exist with Username ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Persona.IfUserExists(@vUsername)", paramlist);

                var status = ErrorStatus.UserSearchChecker(dbResponse.StatusCode);

                if (status.FirstError.Code == "User.UserNotFound")
                {
                    _logger.LogInformation("User does not exist. Creating a new record.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Persona.register_user(@vUsername, @vFullName, @vEmail, @vAddress, @vRegisterDate)", paramlist);

                    if (dbResponse.StatusCode == (int)EnumStatus.Success)
                    {
                        _logger.LogInformation("User successfully created. Returning User details.");
                        return JsonSerializer.Deserialize<UserDTO>(dbResponse.Details, options: null);
                    }
                    else
                    {
                        return Error.Failure(
                                code: dbResponse.Status,
                                description: dbResponse.Details);
                    }
                }
                else
                    return status;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - User creation exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<UserRecord>> GetUserById(GetUserDetailsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving details of {User} at {DateTime}", query.Id, DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing Parameters at {DateTime}", DateTime.Now);

                var user = new User
                {
                    Id = query.Id
                };

                var paramlist = ParamPrep.PrepUserParams(user);

                _logger.LogInformation("Checking if User Exist with Username ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Persona.IfUserExists(@vId)", paramlist);

                var status = ErrorStatus.UserSearchChecker(dbResponse.StatusCode);

                if (status.FirstError.Code == "User.UserExists")
                {
                    _logger.LogInformation("User does not exist. Creating a new record.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Persona.get_user_details(@vId)", paramlist);

                    if (dbResponse.StatusCode == (int)EnumStatus.Success)
                    {
                        _logger.LogInformation("User successfully created. Returning User details.");
                        return JsonSerializer.Deserialize<UserRecord>(dbResponse.Details, options: null);
                    }
                    else
                    {
                        return Error.Failure(
                                code: dbResponse.Status,
                                description: dbResponse.Details);
                    }
                }
                else
                    return Error.Failure(
                        code: status.Errors.FirstOrDefault().Code,
                        description: "User does not exist"
                      );
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - User creation exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public Task<ErrorOr<UserDTO>> UpdateUser(User command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CheckoutDTO> BooksNotReturned(GetBooksNotReturnedQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CheckoutDTO> ReturnBooks(ReturnBooksCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<CheckoutRecord>> CheckoutBooks(CheckoutBooksCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking out Books at {DateTime}", DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing checkout parameters at {DateTime}", DateTime.Now);
                var paramlist = ParamPrep.PrepCheckoutParams(command._checkoutDetails);

                _logger.LogInformation("Checking if User Exists");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Persona.IfUserExists(@vUser)", paramlist);

                var status = ErrorStatus.UserSearchChecker(dbResponse.StatusCode);

                if (status.FirstError.Code == "User.UserExists")
                {
                    _logger.LogInformation("Book does not exist, adding book to the database. Creating a new record.");
                    dbResponse = await connection.QuerySingleAsync<DbResponse>("Select * from Account.checkout_books(@User, @vBookList)", paramlist);

                    if (dbResponse.StatusCode == (int)EnumStatus.Success)
                    {
                        _logger.LogInformation("Book added successfully. Returning book details.");
                        return JsonSerializer.Deserialize<CheckoutRecord>(dbResponse.Details, options: null);
                    }
                    else
                    {
                        return Error.Failure(
                                code: dbResponse.Status,
                                description: dbResponse.Details);
                    }
                }
                else
                    return Error.Validation(
                            code: dbResponse.Status,
                            description: dbResponse.Details);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - Checkout exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);
            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<(string title, string Genre)>> GetTitleGenre(string Isbn)
    {
        try
        {
            _logger.LogInformation("Retrieving book details at {DateTime}", DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing book parameters at {DateTime}", DateTime.Now);

                var book = new Book
                {
                    ISBN = Isbn
                };

                var paramlist = ParamPrep.PrepBookParams(book);

                _logger.LogInformation("Checking if Book Exist with ISBN ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookFound")
                {
                    _logger.LogInformation("Book does not exist, getting Title and Genre.....");
                    var bookRet = JsonSerializer.Deserialize<BookDTO>(dbResponse.Details, options: null);

                    return (bookRet.Title, bookRet.Genre);
                }
                else
                {
                    return Error.Failure(
                        code: "Book does not exists",
                        description: status.Errors.FirstOrDefault().Description
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);

            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }

    public async Task<ErrorOr<bool>> CheckBookEsists(string Isbn)
    {
        try
        {
            _logger.LogInformation("Checking book details at {DateTime}", DateTime.Now);

            using (var connection = await _dataSource.CreateConnection())
            {
                _logger.LogInformation("Preparing book parameters at {DateTime}", DateTime.Now);

                var book = new Book
                {
                    ISBN = Isbn
                };

                var paramlist = ParamPrep.PrepBookParams(book);

                _logger.LogInformation("Checking if Book Exist with ISBN ");
                var dbResponse = await connection.QueryFirstAsync<DbResponse>("Select * from Shelve.IfBookExists(@vISBN)", paramlist);

                var status = ErrorStatus.CheckBookExists(dbResponse.StatusCode);

                if (status.FirstError.Code == "Book.BookFound")
                    return true;
                else
                {
                    return Error.Failure(
                        code: $"Book with ISBN {Isbn} not found",
                        description: status.Errors.FirstOrDefault().Description
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("LibraryRepository - book exception occurred: {ExceptionMessage} at {DateTimeCalled}", ex.Message, DateTime.Now);

            return Error.Failure(
                    code: "Operation Exception",
                    description: ex.Message);
        }
        finally
        {
            _logger.LogInformation("Closing connection at {DateTime}", DateTime.Now);
        }
    }
}
