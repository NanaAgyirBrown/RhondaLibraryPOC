using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.Interfaces.Persistence;
using RhondaLibraryPOC.Infrastructure.Common;
using RhondaLibraryPOC.UnitTest.DataMock;
using System.Data;
using System.Text.Json;

namespace RhondaLibraryPOC.Persistence.Repositories.Tests
{
    public class LibraryRepositoryTests
    {
        private readonly Mock<ILogger<LibraryRepository>> _loggerMock;
        private readonly Mock<IDataSource> _dataSourceMock;

        public LibraryRepositoryTests()
        {
            _loggerMock = new Mock<ILogger<LibraryRepository>>();
            _dataSourceMock = new Mock<IDataSource>();
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
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
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
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
        }

        // Add similar tests for other methods...

        private LibraryRepository CreateRepositoryWithConnection<T>() where T : class
        {
            var repository = new LibraryRepository(_loggerMock.Object, _dataSourceMock.Object);
            _dataSourceMock.Setup(x => x.CreateConnection()).Returns(() => Task.FromResult(Mock.Of<T>()));
            return repository;
        }

        private void ConfigureConnectionMock(LibraryRepository repository, DbResponse dbResponse)
        {
            var connectionMock = new Mock<IDbConnection>();
            _dataSourceMock.Setup(x => x.CreateConnection()).ReturnsAsync(connectionMock.Object);
            connectionMock.Setup(x => x.QueryFirstAsync<DbResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .ReturnsAsync(dbResponse);
        }

        private class BookDTOEqualityComparer : IEqualityComparer<BookDTO>
        {
            public bool Equals(BookDTO x, BookDTO y)
            {
                return x.ISBN == y.ISBN &&
                       x.Title == y.Title &&
                       x.Author == y.Author &&
                       x.Publisher == y.Publisher &&
                       x.Genre == y.Genre &&
                       x.IsAvailable == y.IsAvailable;
            }

            public int GetHashCode(BookDTO obj)
            {
                return obj.ISBN.GetHashCode();
            }
        }
    }
}
