using ErrorOr;

namespace RhondaLibraryPOC.Application.Interfaces
{
    public interface IExtrasRepository
    {
        public Task<ErrorOr<(string title, string Genre)>> GetTitleGenre(string Isbn);
        public Task<ErrorOr<bool>> CheckBookEsists(string Isbn);        
    }
}
