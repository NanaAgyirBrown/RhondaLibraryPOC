using Microsoft.Extensions.Configuration;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Extras;

/// <summary>
/// PICKS UP THE SETTINGS FROM THE APPSETTINGS.JSON FILE
/// </summary>

public class CheckoutSettings
{
    public const string SectionName = "CheckoutSettings";

    public CheckoutSettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();


        MaxBooks = configuration.GetValue<int>($"{SectionName}:MaxBook");
        ReadPeriod = configuration.GetSection($"{SectionName}:{ReadPeriod.SectionName}").Get<ReadPeriod>();
        OverdueFineCharges = configuration.GetSection($"{SectionName}:{OverdueFineCharges.SectionName}").Get<OverdueFineCharges>();
    }

    public int MaxBooks { get; set; }
    public ReadPeriod ReadPeriod { get; set; }
    public OverdueFineCharges OverdueFineCharges { get; set; }

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