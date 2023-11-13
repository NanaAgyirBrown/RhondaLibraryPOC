using System.Data;
using System.Text.Json;
using Dapper;
using ErrorOr;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.Interfaces.Persistence;
using RhondaLibraryPOC.Domain.Entity;
using RhondaLibraryPOC.Infrastructure.Common;
using RhondaLibraryPOC.Persistence.Repositories;
using RhondaLibraryPOC.UnitTest.DataMock;
using Shouldly;

namespace RhondaLibraryPOC.UnitTest.RepositoryMocks
{
    public class LibraryRepositoryTests
    {
        private readonly ILogger<LibraryRepository> _logger;
        private readonly IDataSource _dataSource;

        public LibraryRepositoryTests()
        {
            // Use FakeItEasy for ILogger
            _logger = A.Fake<ILogger<LibraryRepository>>();
            _dataSource = A.Fake<IDataSource>();
        }

        [Fact]
        public async Task AddBook_ValidBook_ReturnsBookDTO()
        {
            // Arrange
            var repository = CreateRepositoryWithConnection<IDbConnection>();

            var cancellationToken = new CancellationToken();
            var validBook = MockData.GetSampleBook();

            var dbResponse = new DbResponse
            {
                StatusCode = (int)EnumStatus.Success,
                Details = JsonSerializer.Serialize(validBook)
            };

            ConfigureConnectionMock(repository, dbResponse);

            // Act
            var result = await repository.AddBook(validBook, cancellationToken);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBe(validBook, ignoreOrder: true);
        }

        [Fact]
        public async Task GetBooks_ReturnsListOfBookDTO()
        {
            // Arrange
            var repository = CreateRepositoryWithConnection<IDbConnection>();

            var cancellationToken = new CancellationToken();

            var dbResponse = new DbResponse
            {
                StatusCode = (int)EnumStatus.Success,
                Details = JsonSerializer.Serialize(MockData.GetBookList())
            };

            ConfigureConnectionMock(repository, dbResponse);

            // Act
            var result = await repository.GetBooks(cancellationToken);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetBookById_ExistingBook_ReturnsBookDTO()
        {
            // Arrange
            var query = new GetBookDetailsQuery { Isbn = MockData.GetRandomBook().ISBN };
            var repository = new LibraryRepository(_logger, _dataSource);

            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(A.Fake<IDbConnection>()));

            A.CallTo(() => A.Fake<IDbConnection>().QueryFirstAsync<DbResponse>(A<string>._, A<object>._, A<CancellationToken>._))
                .Returns(
                    new DbResponse
                    {
                        StatusCode = (int)EnumStatus.Success,
                        Details = JsonSerializer.Serialize(MockData.GetBookFirstDefault(query.Isbn))
                    });

            // Act
            var result = await repository.GetBookById(query, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<ErrorOr<BookDTO>>();

            var bookDto = result.Value;
            bookDto.Title.ShouldBe(MockData.GetBookFirstDefault(query.Isbn).Title);
            bookDto.Genre.ShouldBe(MockData.GetBookFirstDefault(query.Isbn).Genre);
        }

        [Fact]
        public async Task GetBookById_NonExistingBook_ReturnsNotFound()
        {
            // Arrange
            var query = new GetBookDetailsQuery { Isbn = "88888" };
            var repository = new LibraryRepository(_logger, _dataSource);

            // Faking the behavior of CreateConnection and other necessary dependencies
            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(A.Fake<IDbConnection>()));

            A.CallTo(() => A.Fake<IDbConnection>().QueryFirstAsync<DbResponse>(A<string>._, A<object>._, A<CancellationToken>._))
                .Returns(
                    new DbResponse
                    {
                        StatusCode = string.IsNullOrEmpty(MockData.GetBookFirstDefault(query.Isbn).ISBN) ? (int)EnumStatus.NotFound : (int)EnumStatus.NotFound,
                        Details = "Book.BookNotFound"
                    });

            // Act
            var result = await repository.GetBookById(query, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<ErrorOr<BookDTO>>();
            result.IsError.ShouldBeTrue();
            result.Errors.FirstOrDefault().Code.ShouldBe("Book.BookNotFound");
        }

        [Fact]
        public async Task CheckoutBooks_WhenUserExists_ShouldReturnCheckoutRecord()
        {
            // Arrange
            var repository = CreateRepositoryWithConnection<IDbConnection>();

            var command = new CheckoutBooksCommand(
                new Application.CQRS.Checkouts.CheckoutBookList
                {
                    Userid = MockData.GetRandomUser(),
                    CheckoutDate = DateTime.Now,
                    BookList = MockData.GetCheckoutBooks().AsEnumerable()
                });
            var cancellationToken = new CancellationToken();

            // Mocking the behavior of CreateConnection and other necessary dependencies
            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(A.Fake<IDbConnection>()));
            A.CallTo(() => _dataSource.PrepCheckoutParams(A<CheckoutDetails>._)).Returns(/* provide the expected result */);

            // TODO: Mock the behavior of IDbConnection.QueryFirstAsync for user existence check
            A.CallTo(() => A.Fake<IDbConnection>().QueryFirstAsync<DbResponse>(A<string>._, A<object>._, A<CancellationToken>._))
                .ReturnsAsync(new DbResponse(/* provide expected results for user existence check */));

            // TODO: Mock the behavior of IDbConnection.QuerySingleAsync for book addition
            A.CallTo(() => A.Fake<IDbConnection>().QuerySingleAsync<DbResponse>(A<string>._, A<object>._, A<CancellationToken>._))
                .ReturnsAsync(new DbResponse(/* provide expected results for book addition */));

            // Act
            var result = await repository.CheckoutBooks(command, cancellationToken);

            // Assert
            // TODO: Verify the expected results or behaviors based on your actual implementation
            // Example:
            result.Value.ShouldNotBeNull();
            // Add more assertions based on your actual implementation
        }

        [Fact]
        public async Task GetTitleGenre_WhenBookExists_ShouldReturnTitleAndGenre()
        {
            // Arrange
            var repository = CreateRepositoryWithConnection<IDbConnection>();

            var isbn = MockData.GetRandomBook().ISBN;

            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(A.Fake<IDbConnection>()));
            A.CallTo(() => _dataSource.PrepBookParams(A<Book>._)).Returns(/* provide the expected result */);

            // TODO: Mock the behavior of IDbConnection.QueryFirstAsync for book existence check
            A.CallTo(() => A.Fake<IDbConnection>().QueryFirstAsync<DbResponse>(A<string>._, A<object>._, A<CancellationToken>._))
                .ReturnsAsync(new DbResponse(/* provide expected results for book existence check */));

            // TODO: Mock the behavior of IDbConnection.QueryFirstAsync for getting book details
            A.CallTo(() => A.Fake<IDbConnection>().QueryFirstAsync<BookDTO>(A<string>._, A<object>._, A<CancellationToken>._))
                .ReturnsAsync(new BookDTO(/* provide expected results for getting book details */));

            // Act
            var result = await repository.GetTitleGenre(isbn);

            // Assert
            // TODO: Verify the expected results or behaviors based on your actual implementation
            // Example:
            result.Value.title.ShouldNotBeNull();
            result.Value.Genre.ShouldNotBeNull();
            // Add more assertions based on your actual implementation
        }

        private LibraryRepository CreateRepositoryWithConnection<T>() where T : class
        {
            var repository = new LibraryRepository(_logger, _dataSource);
            var connectionMock = A.Fake<T>();

            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(connectionMock));
            return repository;
        }

        private void ConfigureConnectionMock(LibraryRepository repository, DbResponse dbResponse)
        {
            var connectionMock = A.Fake<IDbConnection>();
            A.CallTo(() => _dataSource.CreateConnection()).Returns(Task.FromResult(connectionMock));
            A.CallTo(() => connectionMock.QueryFirstAsync<DbResponse>(A<string>._, A<object>._, A<IDbTransaction>._, A<int?>._, A<CommandType?>._))
                .Returns(Task.FromResult(dbResponse));
        }
    }
}
