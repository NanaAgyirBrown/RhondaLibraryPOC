
using ErrorOr;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

public class Extras
{
    private readonly IExtrasRepository _extrasRepository;

    public Extras(IExtrasRepository extrasRepository)
    {
        _extrasRepository = extrasRepository;
    }

    public async Task<DateTime> CalculateReturnDate(DateTime checkoutDate, string isbn)
    {
        int extraData = await ReadExtraData(isbn);
        
        return checkoutDate.AddDays(extraData);;
    }

    public static decimal IsOverdue(DateTime returnDate)
    {
        if(returnDate <= DateTime.Now)
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

    private static decimal CalculateFine(DateTime returnDate)
    {
        return CalculateFineAmount(returnDate) * (decimal) (DateTime.Now - returnDate).TotalDays;
    }

    private static decimal CalculateFineAmount(DateTime returnDate)
    {
        double daysOverdue = (DateTime.Now - returnDate).TotalDays;

        return daysOverdue switch
        {
            var days when days <= 7 => CheckoutSettings.OverdueFineCharges.FirstWeek,
            var days when days <= 14 => CheckoutSettings.OverdueFineCharges.SecondWeek,
            var days when days <= 21 => CheckoutSettings.OverdueFineCharges.ThirdWeek,
            var days when days > 21 => CheckoutSettings.OverdueFineCharges.Forever,
            _ => 0.00m
        };
    }

    private async Task<int> ReadExtraData(string Isbn)
    {
        var result = await _extrasRepository.GetTitleGenre(Isbn);

        if(string.IsNullOrEmpty(result.Value.Genre))
        {
            return result.Value.Genre switch
            {
                "Fiction" => CheckoutSettings.ReadPeriod.Fiction,
                "Non-Fiction" => CheckoutSettings.ReadPeriod.NonFiction,
                "Reference" => CheckoutSettings.ReadPeriod.Reference,
                _ => CheckoutSettings.ReadPeriod.Others
            };
        }

        return CheckoutSettings.ReadPeriod.Others;
    }
}
