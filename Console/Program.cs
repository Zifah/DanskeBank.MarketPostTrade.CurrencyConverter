using Application;
using Domain;

const string ExitPrompt = "Press any key to exit.";



const int MainCurrencyVolume = 100;
Dictionary<string, decimal> ratesMainToDKK = new()
        {
            { CurrencyISOCodes.EuroISO, 743.94m/MainCurrencyVolume },
            { CurrencyISOCodes.UsDollarISO, 663.11m/MainCurrencyVolume },
            { CurrencyISOCodes.BritishPoundISO, 852.85m / MainCurrencyVolume },
            { CurrencyISOCodes.SwedishKronaISO, 76.10m / MainCurrencyVolume },
            { CurrencyISOCodes.NorwegianKroneISO, 78.40m / MainCurrencyVolume },
            { CurrencyISOCodes.SwissFrancISO,683.58m / MainCurrencyVolume },
            { CurrencyISOCodes.JapaneseYenISO, 5.9740m / MainCurrencyVolume }
        };

ICurrencyConverter converter = new DanishKroneBaseCurrencyConverter(ratesMainToDKK);

var command = Start();
try
{
    (var currencyPair, var amount) = ParseInput();
    var convertedValue = converter.Convert(currencyPair, amount);
    var currencyPairHolder = CurrencyPair.Build(currencyPair);
    Console.WriteLine($"RESULT: {amount} {currencyPairHolder.MainCurrency} = {convertedValue} {currencyPairHolder.MoneyCurrency}");
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}
catch (Exception)
{
    // TODO: Log exception
    Console.WriteLine("Sorry. A problem we did not foresee has happened. We will get right back to you with more information.");
}
Finish();

(string currencyPair, decimal amount) ParseInput()
{
    var parts = command?.Split(' ');

    var invalidCommandError = "The command must be entered according to the specified format.";

    if (parts == null || parts.Length != 2)
    {
        throw new ArgumentException(invalidCommandError);
    }

    var currencyPair = parts[0].Trim();
    var isValidDecimal = decimal.TryParse(parts[1].Trim(), out decimal amount);

    if (!isValidDecimal)
    {
        throw new ArgumentException("The amount to exchange must be a valid currency value.");
    }

    return (currencyPair, amount);
}

static string? Start()
{
    Console.WriteLine("Usage: <currency pair> <amount to exchange>");
    Console.WriteLine("Example: EUR/DKK 25.50");
    return Console.ReadLine();
}

static void Finish()
{
    Console.WriteLine(ExitPrompt);
    Console.Read();
}