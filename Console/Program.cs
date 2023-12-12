using Application;
using Domain;

const string ExitPrompt = "Press any key to exit.";

string currencyPair;
decimal amount;

ICurrencyConverter converter = new DanishKroneBaseCurrencyConverter();

var command = Start();
try
{
    (currencyPair, amount) = ParseInput();
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