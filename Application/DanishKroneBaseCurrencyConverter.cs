using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public class DanishKroneBaseCurrencyConverter : ICurrencyConverter
{
    private const int DecimalPlaces = 4;

    private const int MainCurrencyVolume = 100;
    private const string EuroISO = "EUR";
    private const string USDollarISO = "USD";
    private const string BritishPoundISO = "GBP";
    private const string SwedishKronaISO = "SEK";
    private const string NorwegianKroneISO = "NOK";
    private const string SwissFrancISO = "CHF";
    private const string JapaneseYenISO = "JPY";
    private const string DanishKroneISO = "DKK";
    private readonly Dictionary<string, ExchangeRate> _exchangeRates;

    public DanishKroneBaseCurrencyConverter()
    {
        var euro = new Currency(EuroISO, "Euro");
        var usDollar = new Currency(USDollarISO, "Amerikanske dollar");
        var britishPound = new Currency(BritishPoundISO, "Britiske pund");
        var swedishKrona = new Currency(SwedishKronaISO, "Svenske kroner");
        var norwegianKrone = new Currency(NorwegianKroneISO, "Norske kroner");
        var swissFranc = new Currency(SwissFrancISO, "Schweiziske franc");
        var japaneseYen = new Currency(JapaneseYenISO, "Japanske yen");
        var danishKrone = new Currency(DanishKroneISO, "Danish Krone");

        var euroToDKK = new ExchangeRate(EuroISO, DanishKroneISO, MainCurrencyVolume, 743.94m);
        var usDollarToDKK = new ExchangeRate(USDollarISO, DanishKroneISO, MainCurrencyVolume, 663.11m);
        var britishPoundToDKK = new ExchangeRate(BritishPoundISO, DanishKroneISO, MainCurrencyVolume, 852.85m);
        var swedishKronaToDKK = new ExchangeRate(SwedishKronaISO, DanishKroneISO, MainCurrencyVolume, 76.10m);
        var norwegianKroneToDKK = new ExchangeRate(NorwegianKroneISO, DanishKroneISO, MainCurrencyVolume, 78.40m);
        var swissFrancToDKK = new ExchangeRate(SwissFrancISO, DanishKroneISO, MainCurrencyVolume, 683.58m);
        var japaneseYenToDKK = new ExchangeRate(JapaneseYenISO, DanishKroneISO, MainCurrencyVolume, 5.9740m);

        var dkkToDkk = new ExchangeRate(DanishKroneISO, DanishKroneISO, MainCurrencyVolume, MainCurrencyVolume);

        _exchangeRates = new Dictionary<string, ExchangeRate>
        {
            { $"{EuroISO}/{DanishKroneISO}", euroToDKK },
            { $"{USDollarISO}/{DanishKroneISO}", usDollarToDKK },
            { $"{BritishPoundISO}/{DanishKroneISO}", britishPoundToDKK },
            { $"{SwedishKronaISO}/{DanishKroneISO}", swedishKronaToDKK },
            { $"{NorwegianKroneISO}/{DanishKroneISO}", norwegianKroneToDKK },
            { $"{SwissFrancISO}/{DanishKroneISO}", swissFrancToDKK },
            { $"{JapaneseYenISO}/{DanishKroneISO}", japaneseYenToDKK },
            { $"{DanishKroneISO}/{DanishKroneISO}", dkkToDkk }
        };
    }

    public decimal Convert(string currencyPairInput, decimal amount)
    {
        var forwardCurrencyPair = CurrencyPair.Build(currencyPairInput);
        _exchangeRates.TryGetValue(forwardCurrencyPair.ToString(), out var exchangeRate);

        if (exchangeRate == null && forwardCurrencyPair.MainCurrency == DanishKroneISO)
        {
            exchangeRate = GetReverseExchangeRate(forwardCurrencyPair);
        }

        if (exchangeRate == null)
        {
            throw new ArgumentException($"No rates available for one or more of the currencies in the provided pair: {currencyPairInput}");
        }

        var exchangedAmount = amount * exchangeRate.MoneyCurrencyValue / exchangeRate.MainCurrencyVolume;
        return Decimal.Round(exchangedAmount, DecimalPlaces);
    }

    private ExchangeRate? GetReverseExchangeRate(CurrencyPair forwardCurrencyPair)
    {
        var reverseCurrencyPair = new CurrencyPair(forwardCurrencyPair.MoneyCurrency, forwardCurrencyPair.MainCurrency);
        _exchangeRates.TryGetValue(reverseCurrencyPair.ToString(), out var reverseExchangeRate);

        if (reverseExchangeRate == null)
        {
            return null;
        }

        return new ExchangeRate(
                reverseCurrencyPair.MainCurrency,
                reverseCurrencyPair.MoneyCurrency,
                1,
                reverseExchangeRate.MainCurrencyVolume / reverseExchangeRate.MoneyCurrencyValue);
    }
}
