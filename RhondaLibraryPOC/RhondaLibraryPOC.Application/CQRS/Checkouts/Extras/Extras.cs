
using ErrorOr;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

public class Extras
{
    private readonly IExtrasRepository _extrasRepository;
    private readonly CheckoutSettings _checkoutSettings;

    public Extras(IExtrasRepository extrasRepository)
    {
        _extrasRepository = extrasRepository;
        _checkoutSettings = new CheckoutSettings();
    }

    public int GetMaxCheckoutBooks()
    {
        return _checkoutSettings.MaxBooks;
    }

    public async Task<DateTime> CalculateReturnDate(DateTime checkoutDate, string isbn)
    {
        int extraData = await ReadExtraData(isbn);

        return checkoutDate.AddDays(extraData); ;
    }

    public decimal IsOverdue(DateTime returnDate)
    {
        if (returnDate <= DateTime.Now)
        {
            return CalculateFine(returnDate);
        }

        return 0.00m;
    }

    public async Task<ErrorOr<bool>> GetBookExists(string isbn, CancellationToken cancellationToken)
    {
        var result = await _extrasRepository.CheckBookEsists(isbn);

        return result;
    }

    public async Task<ErrorOr<BookDetail>> GetBookCheckedOut(string user, string book)
    {
        var result = await _extrasRepository.GetCheckedbookDetails(user, book);

        if (result.IsError)
        {
            return Error.Validation(
                    code: $"BookDoesNotExist",
                    description: $"Book with ISBN {book} does not exist"
                                                                 );
        }

        return result;
    }

    private decimal CalculateFine(DateTime returnDate)
    {
        return CalculateFineAmount(returnDate) * (decimal)(DateTime.Now - returnDate).TotalDays;
    }

    private decimal CalculateFineAmount(DateTime returnDate)
    {
        double daysOverdue = (DateTime.Now - returnDate).TotalDays;

        return daysOverdue switch
        {
            var days when days <= 7 => _checkoutSettings.OverdueFineCharges.FirstWeek,
            var days when days <= 14 => _checkoutSettings.OverdueFineCharges.SecondWeek,
            var days when days <= 21 => _checkoutSettings.OverdueFineCharges.ThirdWeek,
            var days when days > 21 => _checkoutSettings.OverdueFineCharges.Forever,
            _ => 0.00m
        };
    }

    private async Task<int> ReadExtraData(string Isbn)
    {
        var result = await _extrasRepository.GetTitleGenre(Isbn);

        if (string.IsNullOrEmpty(result.Value.Genre))
        {
            return result.Value.Genre switch
            {
                "Fiction" => _checkoutSettings.ReadPeriod.Fiction,
                "Non-Fiction" => _checkoutSettings.ReadPeriod.NonFiction,
                "Reference" => _checkoutSettings.ReadPeriod.Reference,
                _ => _checkoutSettings.ReadPeriod.Others
            };
        }

        return _checkoutSettings.ReadPeriod.Others;
    }
}
