using Application;
using Domain;

Console.WriteLine("Usage: <currency pair> <amount to exchange>");
Console.WriteLine("Example: EUR/DKK 25.50");
const string ExitPrompt = "Press any key to exit.";
var command = Console.ReadLine();

string currencyPair;
decimal amount;

try
{
    (currencyPair, amount) = ParseInput();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Finish();
    return;
}

ICurrencyConverter converter = new DanishKroneBaseCurrencyConverter();
try
{
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

static void Finish()
{
    Console.WriteLine(ExitPrompt);
    Console.Read();
}