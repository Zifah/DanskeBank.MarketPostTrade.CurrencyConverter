using AutoFixture;
using AutoFixture.Xunit2;
using Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Application.UnitTests
{
    public class DanishKroneBaseCurrencyConverterTests
    {
        private DanishKroneBaseCurrencyConverter _theConverter;

        private const int DecimalPlaces = 4;
        private const int MainCurrencyVolume = 100;

        // Ideally would be auto-generated but I don't have more time.
        private static readonly Dictionary<string, decimal> _ratesMainToDKK = new()
        {
            { CurrencyISOCodes.EuroISO, 743.94m/MainCurrencyVolume },
            { CurrencyISOCodes.UsDollarISO, 663.11m/MainCurrencyVolume },
            { CurrencyISOCodes.BritishPoundISO, 852.85m / MainCurrencyVolume },
            { CurrencyISOCodes.SwedishKronaISO, 76.10m / MainCurrencyVolume },
            { CurrencyISOCodes.NorwegianKroneISO, 78.40m / MainCurrencyVolume },
            { CurrencyISOCodes.SwissFrancISO,683.58m / MainCurrencyVolume },
            { CurrencyISOCodes.JapaneseYenISO, 5.9740m / MainCurrencyVolume }
        };


        private static Fixture AutoFixture
        {
            get
            {
                var fixture = new Fixture();
                fixture.Customize<decimal>(c => c.FromFactory(() =>
                {
                    var random = new Random();
                    var value = Math.Round((decimal)random.NextDouble() * 100, 2);
                    return value;
                }));
                return fixture;
            }
        }

        public DanishKroneBaseCurrencyConverterTests()
        {
            _theConverter = new DanishKroneBaseCurrencyConverter(_ratesMainToDKK);
        }

        [Theory]
        [AutoData]
        public void Convert_WhenDKKtoDKK_ReturnsInputAmount(decimal mainAmount)
        {
            // Arrange
            var currencyPair = "DKK/DKK";

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainAmount);

            // Assert
            exchangedAmount.ShouldBe(mainAmount);
        }

        [Theory]
        [InlineData("DKK/DKK")]
        [InlineData("dkk/dkk")]
        public void Convert_AllowsUppercaseOrLowerCase(string currencyPair)
        {
            // Arrange
            var mainAmount = AutoFixture.Create<decimal>();

            // Act
            var action = () => _theConverter.Convert(currencyPair, mainAmount);

            // Assert
            action.ShouldNotThrow();

        }

        [Theory]
        [InlineData(CurrencyISOCodes.EuroISO)]
        [InlineData(CurrencyISOCodes.UsDollarISO)]
        [InlineData(CurrencyISOCodes.BritishPoundISO)]
        [InlineData(CurrencyISOCodes.SwedishKronaISO)]
        [InlineData(CurrencyISOCodes.NorwegianKroneISO)]
        [InlineData(CurrencyISOCodes.SwissFrancISO)]
        [InlineData(CurrencyISOCodes.JapaneseYenISO)]
        public void Convert_WhenAnyConfiguredCurrencyToDKK_ReturnsExpectedAmount(string mainCurrency)
        {
            // Arrange
            var mainAmount = AutoFixture.Create<decimal>();
            var currencyPair = $"{mainCurrency}/{CurrencyISOCodes.DanishKroneISO}";
            var expectedExchangeAmount = decimal.Round(mainAmount * _ratesMainToDKK[mainCurrency], DecimalPlaces);

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainAmount);

            // Assert
            exchangedAmount.ShouldBe(expectedExchangeAmount);
        }

        [Theory]
        [InlineData(CurrencyISOCodes.EuroISO)]
        [InlineData(CurrencyISOCodes.UsDollarISO)]
        [InlineData(CurrencyISOCodes.BritishPoundISO)]
        [InlineData(CurrencyISOCodes.SwedishKronaISO)]
        [InlineData(CurrencyISOCodes.NorwegianKroneISO)]
        [InlineData(CurrencyISOCodes.SwissFrancISO)]
        [InlineData(CurrencyISOCodes.JapaneseYenISO)]
        public void Convert_WhenDKKToAnyConfiguredCurrency_ReturnsExpectedAmount(string moneyCurrency)
        {
            // Arrange
            var mainDkkAmount = AutoFixture.Create<decimal>();
            var currencyPair = new CurrencyPair(CurrencyISOCodes.DanishKroneISO, moneyCurrency).ToString();
            var expectedExchangeAmount = decimal.Round(mainDkkAmount / _ratesMainToDKK[moneyCurrency], DecimalPlaces);

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainDkkAmount);

            // Assert
            exchangedAmount.ShouldBe(expectedExchangeAmount, DecimalPlaces);
        }

        private static IEnumerable<object[]> CombineAllNonDkkCurrencies()
        {
            var nonDkkCurrencies = new List<string>
            {
                CurrencyISOCodes.EuroISO,
                CurrencyISOCodes.UsDollarISO,
                CurrencyISOCodes.BritishPoundISO,
                CurrencyISOCodes.SwedishKronaISO,
                CurrencyISOCodes.NorwegianKroneISO,
                CurrencyISOCodes.SwissFrancISO,
                CurrencyISOCodes.JapaneseYenISO
            };

            foreach (var mainCurrency in nonDkkCurrencies)
            {
                foreach (var moneyCurrency in nonDkkCurrencies)
                {
                    if (mainCurrency != moneyCurrency)
                    {
                        // Convert from Main to DKK, and then from DKK to Money currency
                        var inputAmount = AutoFixture.Create<decimal>();
                        var mainToDKKRate = _ratesMainToDKK[mainCurrency];
                        var dkkToMoneyRate = 1 / _ratesMainToDKK[moneyCurrency];
                        var expectedExchangedAmount =
                            decimal.Round(inputAmount * mainToDKKRate * dkkToMoneyRate, DecimalPlaces);

                        yield return new object[] { mainCurrency, moneyCurrency, inputAmount, expectedExchangedAmount };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(CombineAllNonDkkCurrencies))]
        public void Convert_WhenBothCurrenciesConfiguredToDKK_ConvertsCorrectlyBetweenEachOther(
            string mainCurrency,
            string moneyCurrency,
            decimal inputAmount,
            decimal expectedExchangedAmount
            )
        {
            // Act
            var exchangedAmount = _theConverter.Convert(new CurrencyPair(mainCurrency, moneyCurrency).ToString(),
                inputAmount
                );

            // Assert
            exchangedAmount.ShouldBe(expectedExchangedAmount);
        }

        [Theory]
        [InlineAutoData($"{CurrencyISOCodes.EuroISO}/{CurrencyISOCodes.EuroISO}")]
        [InlineAutoData($"{CurrencyISOCodes.DanishKroneISO}/{CurrencyISOCodes.DanishKroneISO}")]
        [InlineAutoData($"{CurrencyISOCodes.SwedishKronaISO}/{CurrencyISOCodes.SwedishKronaISO}")]
        [InlineAutoData($"{CurrencyISOCodes.SwissFrancISO}/{CurrencyISOCodes.SwissFrancISO}")]
        [InlineAutoData($"{CurrencyISOCodes.NorwegianKroneISO}/{CurrencyISOCodes.NorwegianKroneISO}")]
        [InlineAutoData($"{CurrencyISOCodes.JapaneseYenISO}/{CurrencyISOCodes.JapaneseYenISO}")]
        [InlineAutoData($"{CurrencyISOCodes.UsDollarISO}/{CurrencyISOCodes.UsDollarISO}")]
        [InlineAutoData($"{CurrencyISOCodes.BritishPoundISO}/{CurrencyISOCodes.BritishPoundISO}")]

        public void Convert_WhenMainAndMoneyCurrencyAreSame_ReturnsInputAmount(string currencyPair, decimal originalAmount)
        {
            // Act
            var exchangedValue = _theConverter.Convert(currencyPair, originalAmount);

            // Assert
            exchangedValue.ShouldBe(originalAmount);
        }


        [Theory]
        [InlineAutoData("ABC/DEF")]
        [InlineAutoData("GHI/JKL")]
        [InlineAutoData("MNO/PQR")]
        [InlineAutoData("STU/VWXY")]
        public void Convert_WhenInvalidCurrencyPair_ThrowsInformativeException(string currencyPair, decimal amount)
        {
            // Act and assert
            Should
                .Throw<ArgumentException>(() => _theConverter.Convert(currencyPair, amount))
                .Message.ShouldStartWith(ErrorMessages.ExchangeRateNotConfigured);
        }
    }
}