﻿using Domain;
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
        var currencyPair = CurrencyPair.Build(currencyPairInput);

        if (currencyPair.AreSame())
        {
            // TODO Hafiz: Write test case for this
            return amount;
        }

        _exchangeRates.TryGetValue(currencyPair.ToString(), out var exchangeRate);

        if (exchangeRate == null && currencyPair.MainCurrency == DanishKroneISO)
        {
            exchangeRate = TryGetDKKToOtherCurrencyRate(currencyPair.MoneyCurrency);
        }

        exchangeRate ??= TryGetExchangeRateViaDKKIntermediary(currencyPair)
             ?? throw new ArgumentException($"No rates available for one or more of the currencies in the provided pair: {currencyPairInput}");

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
        _exchangeRates.TryGetValue(new CurrencyPair(otherCurrencyRate, DanishKroneISO).ToString(),
            out var moneyToDkkExchangeRate);
        return moneyToDkkExchangeRate?.Invert();
    }

    private ExchangeRate? TryGetExchangeRateViaDKKIntermediary(CurrencyPair currencyPair)
    {
        var mainToDkkCurrencyPair = new CurrencyPair(currencyPair.MainCurrency, DanishKroneISO);
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
