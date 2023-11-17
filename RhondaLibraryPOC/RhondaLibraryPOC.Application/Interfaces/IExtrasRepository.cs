using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Checkouts;

namespace RhondaLibraryPOC.Application.Interfaces
{
    public interface IExtrasRepository
    {
        public Task<ErrorOr<(string title, string Genre)>> GetTitleGenre(string Isbn);
        public Task<ErrorOr<bool>> CheckBookEsists(string Isbn);
        public Task<ErrorOr<BookDetail>> GetCheckedOutbook(string User, string BookId);
        public Task<ErrorOr<BookDetail>> GetCheckedbookDetails(string CheckoutId, string BookId);
    }
}
