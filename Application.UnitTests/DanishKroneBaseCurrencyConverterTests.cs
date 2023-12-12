using AutoFixture;
using AutoFixture.Xunit2;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Application.UnitTests
{
    public class DanishKroneBaseCurrencyConverterTests
    {
        private DanishKroneBaseCurrencyConverter _theConverter;
        private readonly Fixture _fixture;
        private const int DecimalPlaces = 4;
        private const string
            EuroISO = "EUR",
            UsDollarISO = "USD",
            BritishPoundISO = "GBP",
            SwedishKronaISO = "SEK",
            NorwegianKroneISO = "NOK",
            SwissFrancISO = "CHF",
            JapaneseYenISO = "JPY",
            DanishKroneISO = "DKK";
        private static Dictionary<string, decimal> _rates100UnitsMainToDKK = new Dictionary<string, decimal>()
        {
            { EuroISO, 743.94m },
            { UsDollarISO, 663.11m },
            { BritishPoundISO, 852.85m },
            { SwedishKronaISO, 76.10m },
            { NorwegianKroneISO, 78.40m },
            { SwissFrancISO,683.58m },
            { JapaneseYenISO, 5.9740m },
            { DanishKroneISO, 100m },

        };

        public DanishKroneBaseCurrencyConverterTests()
        {
            _theConverter = new DanishKroneBaseCurrencyConverter();
            _fixture = new Fixture();
            _fixture.Customize<decimal>(c => c.FromFactory(() =>
            {
                var random = new Random();
                var value = Math.Round((decimal)random.NextDouble() * 100, 2);
                return value;
            }));
        }

        private IEnumerable<object[]> GetCurrencyConversionTestData()
        {
            var currencies = new List<string> { EuroISO, UsDollarISO, BritishPoundISO, SwedishKronaISO, NorwegianKroneISO, SwissFrancISO, JapaneseYenISO };

            foreach (var sourceCurrency in currencies)
            {
                foreach (var destinationCurrency in currencies)
                {
                    if (sourceCurrency != destinationCurrency && destinationCurrency != DanishKroneISO)
                    {
                        var inputValue = _fixture.Create<decimal>(); // Generate random input value
                        var sourceToDKKRate = _rates100UnitsMainToDKK[sourceCurrency];
                        var DKKToDestinationRate = _rates100UnitsMainToDKK[destinationCurrency];
                        var expectedExchangedValue = inputValue * (sourceToDKKRate / DKKToDestinationRate);

                        yield return new object[] { sourceCurrency, destinationCurrency, inputValue, expectedExchangedValue };
                    }
                }
            }
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
            var mainAmount = _fixture.Create<decimal>();

            // Act
            var action = () => _theConverter.Convert(currencyPair, mainAmount);

            // Assert
            action.ShouldNotThrow();

        }

        [Theory]
        [InlineData(EuroISO)]
        [InlineData(UsDollarISO)]
        [InlineData(BritishPoundISO)]
        [InlineData(SwedishKronaISO)]
        [InlineData(NorwegianKroneISO)]
        [InlineData(SwissFrancISO)]
        [InlineData(JapaneseYenISO)]
        public void Convert_WhenAnyConfiguredCurrencyToDKK_ReturnsExpectedAmount(string mainCurrency)
        {
            // Arrange
            var mainAmount = _fixture.Create<decimal>();
            var currencyPair = $"{mainCurrency}/{DanishKroneISO}";
            var expectedExchangeAmount = decimal.Round(
                mainAmount * _rates100UnitsMainToDKK[mainCurrency] / 100, DecimalPlaces);

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainAmount);

            // Assert
            exchangedAmount.ShouldBe(expectedExchangeAmount);
        }

        [Theory]
        [InlineData(EuroISO)]
        [InlineData(UsDollarISO)]
        [InlineData(BritishPoundISO)]
        [InlineData(SwedishKronaISO)]
        [InlineData(NorwegianKroneISO)]
        [InlineData(SwissFrancISO)]
        [InlineData(JapaneseYenISO)]
        public void Convert_WhenDKKToAnyConfiguredCurrency_ReturnsExpectedAmount(string moneyCurrency)
        {
            // Arrange
            var mainDkkAmount = _fixture.Create<decimal>();
            var currencyPair = $"{DanishKroneISO}/{moneyCurrency}";
            var expectedExchangeAmount = decimal.Round(
                mainDkkAmount * 100 / _rates100UnitsMainToDKK[moneyCurrency], DecimalPlaces);

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainDkkAmount);

            // Assert
            exchangedAmount.ShouldBe(expectedExchangeAmount, DecimalPlaces);
        }

        [Theory]
        [AutoData]
        public void Convert_WhenInvalidCurrencyPair_ThrowsInformativeException(string currencyPair)
        {
            _ = currencyPair;
        }

        [Fact]
        public void Convert_WhenBothCurrenciesConfigured_ConvertCorrectlyBetweenEachOther()
        {

        }
    }
}