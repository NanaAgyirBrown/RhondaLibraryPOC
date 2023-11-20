using ErrorOr;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using RhondaLibraryPOC.Presentation.Controllers;
using RhondaLibraryPOC.UnitTest.DataMock;
using System.Net;

namespace RhondaLibraryPOC.UnitTest.ControllersTest
{
    public class BookControllerTests
    {
        private readonly ILogger<BookController> _logger;
        private readonly IMediator _mediator;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            _logger = A.Fake<ILogger<BookController>>();
            _mediator = A.Fake<IMediator>();
            _controller = new BookController(_logger, _mediator);
        }

        [Fact]
        public async Task AddBook_ValidCommand_ReturnsOk()
        {
            // Arrange
            var validCommand = new AddBookCommand(MockData.GetRandomBook());
            var expectedResult = MockData.GetBookFirstDefault(validCommand.BookDTO.ISBN);

            A.CallTo(() => _mediator.Send(validCommand, A<CancellationToken>._)).Returns(expectedResult);

            // Act
            var result = await _controller.AddBook(validCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsFound()
        {
            // Arrange
            ErrorOr<IEnumerable<BookDTO>> expectedBookList = MockData.GetBookList();

            A.CallTo(() => _mediator.Send(A<GetAllBooksQuery>._, A<CancellationToken>._)).Returns(expectedBookList);

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var FoundResult = (OkObjectResult)result;
            FoundResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedBookList.Value);

        }

        [Fact]
        public async Task GetBookByIsbn_ValidQuery_ReturnsOk()
        {
            // Arrange
            var book = MockData.GetRandomBook();
            var validQuery = new GetBookDetailsQuery { Isbn = book.ISBN };

            A.CallTo(() => _mediator.Send(validQuery, A<CancellationToken>._)).Returns(book);

            // Act
            var result = await _controller.GetBookByIsbn(validQuery);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okResult.Value.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task UpdateBook_ValidCommand_ReturnsOk()
        {

            string? Title = "The Hobbit";
            string? Author = "J.R.R. Tolkien";
            string? ISBN = "9780547928227";
            string? Publisher = "Houghton Mifflin Harcourt";
            string? Genre = "Reference";
            bool? IsAvailable = true;

            // Arrange
            var validCommand = new UpdateBookCommand(ISBN, Title, Author, Publisher, Genre, IsAvailable);

            var updatedBook = MockData.UpdateValue((ISBN, Title, Author, Publisher, Genre, IsAvailable.ToString()));

            ErrorOr<BookDTO> expectedBook = updatedBook;

            A.CallTo(() => _mediator.Send(validCommand, A<CancellationToken>._)).Returns(expectedBook);

            // Act
            var result = await _controller.UpdateBook(validCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedBook.Value);
        }

        [Fact]
        public async Task DeleteBook_ValidCommand_ReturnsOk()
        {
            // Arrange
            var validCommand = new DeleteBookCommand { Isbn = "9780547928227" };

            ErrorOr<Application.CQRS.Books.BookDTO> expectedBook = new Application.CQRS.Books.BookDTO
            {
                Title = "The Hobbit",
                Author = "J.R.R. Tolkien",
                ISBN = "9780547928227",
                Publisher = "Houghton Mifflin Harcourt",
                Genre = "Fantasy",
                IsAvailable = true
            };

            A.CallTo(() => _mediator.Send(validCommand, A<CancellationToken>._)).Returns(expectedBook);

            // Act
            var result = await _controller.DeleteBook(validCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedBook.Value);
        }
    }
}
