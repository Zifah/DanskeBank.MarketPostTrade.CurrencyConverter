using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

internal class DanishKroneBaseCurrencyConverter : ICurrencyConverter
{
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

        var euroToDKK = new ExchangeRate(euro, danishKrone, MainCurrencyVolume, 743.94m);
        var usDollarToDKK = new ExchangeRate(usDollar, danishKrone, MainCurrencyVolume, 663.11m);
        var britishPoundToDKK = new ExchangeRate(britishPound, danishKrone, MainCurrencyVolume, 852.85m);
        var swedishKronaToDKK = new ExchangeRate(swedishKrona, danishKrone, MainCurrencyVolume, 76.10m);
        var norwegianKroneToDKK = new ExchangeRate(norwegianKrone, danishKrone, MainCurrencyVolume, 78.40m);
        var swissFrancToDKK = new ExchangeRate(swissFranc, danishKrone, MainCurrencyVolume, 683.58m);
        var japaneseYenToDKK = new ExchangeRate(japaneseYen, danishKrone, MainCurrencyVolume, 5.9740m);

        var dkkToDkk = new ExchangeRate(danishKrone, danishKrone, MainCurrencyVolume, MainCurrencyVolume);

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

    public decimal Convert(string currencyPair, decimal amount)
    {
        throw new NotImplementedException();
    }
}
