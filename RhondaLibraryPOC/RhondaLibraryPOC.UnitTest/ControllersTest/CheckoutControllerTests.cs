using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Presentation.Controllers;
using Shouldly;

namespace RhondaLibraryPOC.Tests.Controllers
{
    public class CheckoutControllerTests
    {
        [Fact]
        public async Task Checkout_ValidRequest_ReturnsOk()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CheckoutController>>();
            var mediatorMock = new Mock<IMediator>();

            var checkoutBookCommand = new CheckoutBookCommand(
                new CheckoutBookList
                {
                    Userid = "123456",
                    BookList = new List<CheckoutBook>
                    {
                        new CheckoutBook
                        {
                            BookId = "9780547928197",
                            CheckoutDate = DateTime.Now,
                            Returned = false,
                            Fine = 0
                        }
                    },
                    CheckoutDate = DateTime.Now
                }
            );

            ErrorOr<UserCheckout> expectedResult = new UserCheckout
            {
                User = new UserDetail
                {
                    Email = "tester@testing.com",
                    UserID = "123456",
                    FullName = "Test"
                },
                Checkouts = new CheckoutDetail
                {
                    CheckoutId = "987456321",
                    Books = new List<BookDetail>
                    {
                        new BookDetail
                        {
                            BookId = "9780547928197",
                            Title = "The Hobbit",
                            Returned = false,
                            CheckoutDate = DateTime.Now,
                            ExpectedReturnDate = DateTime.Now.AddDays(14),
                            Fine = 0
                        }
                    }
                }
            };

            mediatorMock.Setup(x => x.Send(It.IsAny<CheckoutBookCommand>(), default)).ReturnsAsync(expectedResult);

            var controller = new CheckoutController(loggerMock.Object, mediatorMock.Object);

            // Act
            var result = await controller.Checkout(checkoutBookCommand) as ObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var actualResult = result.Value as UserCheckout;
            actualResult.ShouldNotBeNull();
            actualResult.Checkouts.ShouldBe(expectedResult.Value.Checkouts);

        }

        [Fact]
        public async Task Checkout_InvalidRequest_ReturnsNotFound()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CheckoutController>>();
            var mediatorMock = new Mock<IMediator>();

            var checkoutBookCommand = new CheckoutBookCommand(
                new CheckoutBookList
                {
                    Userid = "123Invalid",
                    BookList = new List<CheckoutBook>
                    {
                        new CheckoutBook
                        {
                            BookId = "9780547928197",
                            CheckoutDate = DateTime.Now,
                            Returned = false,
                            Fine = 0
                        }
                    },
                    CheckoutDate = DateTime.Now
                }
            );

            var expectedResult = Error.NotFound(
                code: "404",
                description: "User with ID 123Invalid does not exist"
             );

            mediatorMock.Setup(x => x.Send(It.IsAny<CheckoutBookCommand>(), default)).ReturnsAsync(expectedResult);

            var controller = new CheckoutController(loggerMock.Object, mediatorMock.Object);

            // Act
            var result = await controller.Checkout(checkoutBookCommand) as ObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
            result.Value.ShouldBe("User with ID 123Invalid does not exist");
        }
    }
}
