namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

/// <summary>
/// PICKS UP THE SETTINGS FROM THE APPSETTINGS.JSON FILE
/// </summary>

public static class CheckoutSettings
{
    public const string SectionName = "CheckoutSettings";

    public static int MaxBooks { get; set; }
    public static ReadPeriod? ReadPeriod { get; set; }
    public static OverdueFineCharges? OverdueFineCharges { get; set; }

}

public class OverdueFineCharges
{
    public const string SectionName = "OverdueFineCharges";

    public decimal FirstWeek { get; set; }
    public decimal SecondWeek { get; set; }
    public decimal ThirdWeek { get; set; }
    public decimal Forever { get; set; }
}

public class ReadPeriod
{
    public const string SectionName = "ReadPeriod";

    public int Fiction { get; set; }
    public int NonFiction { get; set; }
    public int Reference { get; set; }
    public int Others { get; set; }
}