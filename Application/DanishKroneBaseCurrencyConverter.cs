using Domain;

namespace Application;

public class DanishKroneBaseCurrencyConverter : ICurrencyConverter
{
    private const int DecimalPlaces = 4;
    private readonly Dictionary<string, ExchangeRate> _exchangeRates;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exchangeRates">
    /// <para>Key: The ISOCode of the main currency.</para>
    /// <para>Value: The amount of DKK that 1 unit of the main currency fetches.</para>
    /// </param>
    public DanishKroneBaseCurrencyConverter(IDictionary<string, decimal> exchangeRates)
    {
        _exchangeRates = exchangeRates.ToDictionary(
            kvp => new CurrencyPair(kvp.Key, CurrencyISOCodes.DanishKroneISO).ToString(), 
            kvp => new ExchangeRate(kvp.Key, CurrencyISOCodes.DanishKroneISO, 1, kvp.Value));
    }

    public decimal Convert(string currencyPairInput, decimal amount)
    {
        var currencyPair = CurrencyPair.Build(currencyPairInput);

        if (currencyPair.AreSame())
        {
            return amount;
        }

        _exchangeRates.TryGetValue(currencyPair.ToString(), out var exchangeRate);

        if (exchangeRate == null && currencyPair.MainCurrency == CurrencyISOCodes.DanishKroneISO)
        {
            exchangeRate = TryGetDKKToOtherCurrencyRate(currencyPair.MoneyCurrency);
        }

        exchangeRate ??= TryGetExchangeRateViaDKKIntermediary(currencyPair)
             ?? throw new ArgumentException($"{ErrorMessages.ExchangeRateNotConfigured}: {currencyPairInput}");

        var exchangedAmount = amount * exchangeRate.MoneyCurrencyValue / exchangeRate.MainCurrencyVolume;
        return decimal.Round(exchangedAmount, DecimalPlaces);
    }

    /// <summary>
    /// Computes the DKK -> OtherCurrency rate from a rate in the other direction (if it exists)
    /// </summary>
    /// <param name="forwardCurrencyPair"></param>
    /// <returns>Returns the result of the computation if the other currency converts to DKK; null otherwise</returns>
    private ExchangeRate? TryGetDKKToOtherCurrencyRate(string otherCurrencyRate)
    {
        _exchangeRates.TryGetValue(new CurrencyPair(otherCurrencyRate, CurrencyISOCodes.DanishKroneISO).ToString(),
            out var moneyToDkkExchangeRate);
        return moneyToDkkExchangeRate?.Invert();
    }

    private ExchangeRate? TryGetExchangeRateViaDKKIntermediary(CurrencyPair currencyPair)
    {
        var mainToDkkCurrencyPair = new CurrencyPair(currencyPair.MainCurrency, CurrencyISOCodes.DanishKroneISO);
        _exchangeRates.TryGetValue(mainToDkkCurrencyPair.ToString(), out var mainToDkkExchangeRate);

        if (mainToDkkExchangeRate == null)
        {
            return null;
        }

        var dkkToMoneyExchangeRate = TryGetDKKToOtherCurrencyRate(currencyPair.MoneyCurrency);

        if (dkkToMoneyExchangeRate == null)
        {
            return null;
        }

        var mainToMoneyRate = mainToDkkExchangeRate.GetSingleUnitRate() * dkkToMoneyExchangeRate.GetSingleUnitRate();
        return new ExchangeRate(
            currencyPair.MainCurrency,
            currencyPair.MoneyCurrency,
            1,
            mainToMoneyRate);
    }
}
