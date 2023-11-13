using MediatR;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhondaLibraryPOC.UnitTest.DataMock;

public class MockData
{
    private static List<CheckoutDTO> CheckoutDTOs = new List<CheckoutDTO>()
    {
        new CheckoutDTO()
        {
            Userid = "1",
            BookId = "1",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "1",
            BookId = "2",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "1",
            BookId = "3",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "1",
            BookId = "4",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "3",
            BookId = "5",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "3",
            BookId = "6",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "1",
            BookId = "7",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "2",
            BookId = "3",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutDTO()
        {
            Userid = "2",
            BookId = "2",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        }
    };

    private static List<CheckoutBook> CheckoutBooks = new List<CheckoutBook>()
    {
        new CheckoutBook()
        {
            BookId = "1",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutBook()
        {
            BookId = "2",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        },
        new CheckoutBook()
        {
            BookId = "3",
            CheckoutDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            Returned = false,
            Fine = 0
        }
    };

    private static List<BookDTO> BookList = new List<BookDTO>()
    {
        new BookDTO()
        {
            Title = "The Hobbit",
            Author = "J.R.R. Tolkien",
            ISBN = "9780547928227",
            Publisher = "Houghton Mifflin Harcourt",
            Genre = "Fantasy",
            IsAvailable = true
        },
        new BookDTO()
        {
            Title = "The Fellowship of the Ring",
            Author = "J.R.R. Tolkien",
            ISBN = "9780547928210",
            Publisher = "Houghton Mifflin Harcourt",
            Genre = "Fantasy",
            IsAvailable = true
        },
        new BookDTO()
        {
            Title = "The Two Towers",
            Author = "J.R.R. Tolkien",
            ISBN = "9780547928203",
            Publisher = "Houghton Mifflin Harcourt",
            Genre = "Fantasy",
            IsAvailable = true
        },
        new BookDTO()
        {
            Title = "The Return of the King",
            Author = "J.R.R. Tolkien",
            ISBN = "9780547928197",
            Publisher = "Houghton Mifflin Harcourt",
            Genre = "Fantasy",
            IsAvailable = true
        }
    };

    private static Book sampleBook = new Book()
    {
        Title = "The Hobbit",
        Author = "J.R.R. Tolkien",
        ISBN = "9780547928227",
        Publisher = "Houghton Mifflin Harcourt",
        Genre = "Fantasy",
        IsAvailable = true
    };

    public static BookDTO GetBookFirstDefault(string Isbn)
    {
        return BookList.FirstOrDefault(x => x.ISBN == Isbn);
    }

    public static List<BookDTO> GetBookList()
    {
        return BookList;
    }

    public static List<CheckoutDTO> GetCheckoutDTOs()
    {
        return CheckoutDTOs;
    }

    public static List<CheckoutBook> GetCheckoutBooks()
    {
        return CheckoutBooks;
    }

    public static BookDTO GetRandomBook()
    {
        Random random = new Random();
        int index = random.Next(BookList.Count);

        return BookList[index];
    }

    public static CheckoutDTO GetRandomCheckoutDTO()
    {
        Random random = new Random();
        int index = random.Next(CheckoutDTOs.Count);

        return CheckoutDTOs[index];
    }

    public static BookDTO UpdateValue((string? ISBN, string? Title, string? Author, string? Publisher, string? Genre, string? IsAvailable) update)
    {
        var book = BookList.FirstOrDefault(x => x.ISBN == update.ISBN);

        book.Title = update.Title;
        book.Author = update.Author;
        book.Publisher = update.Publisher;
        book.Genre = update.Genre;
        book.IsAvailable = bool.Parse(update.IsAvailable);

        return book;
    }

    public static Book GetSampleBook() => sampleBook;

    internal static string GetRandomUser()
    {
        Random random = new Random();
        int index = random.Next(CheckoutDTOs.Count);

        return CheckoutDTOs[index].User.Id;
    }
} 