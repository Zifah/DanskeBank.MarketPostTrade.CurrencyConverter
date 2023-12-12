namespace Application;

internal interface ICurrencyConverter
{
    /// <summary>
    /// Converts the <paramref name="amount"/> from the MainCurrency to the MoneyCurrency
    /// </summary>
    /// <param name="currencyPair">Should be in the format: MainCurrency/MoneyCurrency (case insensitive)</param>
    /// <param name="amount">The amount in the MainCurrency to be converted to </param>
    /// <returns>The result of the currency conversion to 4 decimal places.</returns>
    /// <exception cref="ArgumentException">
    /// This exception is thrown in two cases:
    /// 1. The currency pair is not valid
    /// 2. No exchange rates were found for the currency pair
    /// </exception>
    decimal Convert(string currencyPair, decimal amount);
}
