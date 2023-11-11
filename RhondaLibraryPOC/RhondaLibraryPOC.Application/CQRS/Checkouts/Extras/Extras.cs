
namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

internal static class Extras
{
    internal static DateTime CalculateReturnDate(DateTime checkoutDate, string Genre)
    {
        return checkoutDate.AddDays(ReadPeriod(Genre));
    }

    internal static decimal IsOverdue(DateTime returnDate)
    {
        if(returnDate <= DateTime.Now)
        {
            return CalculateFine(returnDate);
        }

        return 0.00m;
    }

    static decimal CalculateFine(DateTime returnDate)
    {
        return CalculateFineAmount(returnDate) * 0.25m;
    }

    private static decimal CalculateFineAmount(DateTime returnDate)
    {
        double daysOverdue = (DateTime.Now - returnDate).TotalDays;

        return daysOverdue switch
        {
            var days when days <= 7 => 0.25m,
            var days when days <= 14 => 1.50m,
            var days when days <= 21 => 3.75m,
            var days when days > 21 => 15.00m,
            _ => 0.25m
        };
    }

    static int ReadPeriod(string genre)
    {
        return genre switch
        {
            "Fiction" => 7,
            "Non-Fiction" => 14,
            "Reference" => 7,
            _ => 14
        };
    }
}
