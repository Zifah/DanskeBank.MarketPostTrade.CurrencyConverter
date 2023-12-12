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

        private const int DecimalPlaces = 4;
        private const int MainCurrencyVolume = 100;
        private const string
            EuroISO = "EUR",
            UsDollarISO = "USD",
            BritishPoundISO = "GBP",
            SwedishKronaISO = "SEK",
            NorwegianKroneISO = "NOK",
            SwissFrancISO = "CHF",
            JapaneseYenISO = "JPY",
            DanishKroneISO = "DKK";

        // TODO: Pass the rates to the converter instead of duplicating the data between test and implementation
        private static Dictionary<string, decimal> _rates100UnitsMainToDKK = new Dictionary<string, decimal>()
        {
            { EuroISO, 743.94m },
            { UsDollarISO, 663.11m },
            { BritishPoundISO, 852.85m },
            { SwedishKronaISO, 76.10m },
            { NorwegianKroneISO, 78.40m },
            { SwissFrancISO,683.58m },
            { JapaneseYenISO, 5.9740m },
            { DanishKroneISO, MainCurrencyVolume }
        };

        public DanishKroneBaseCurrencyConverterTests()
        {
            _theConverter = new DanishKroneBaseCurrencyConverter();
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
            var mainAmount = AutoFixture.Create<decimal>();
            var currencyPair = $"{mainCurrency}/{DanishKroneISO}";
            var expectedExchangeAmount = decimal.Round(
                mainAmount * _rates100UnitsMainToDKK[mainCurrency] / MainCurrencyVolume, DecimalPlaces);

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
            var mainDkkAmount = AutoFixture.Create<decimal>();
            var currencyPair = new CurrencyPair(DanishKroneISO, moneyCurrency).ToString();
            var expectedExchangeAmount = decimal.Round(
                mainDkkAmount * MainCurrencyVolume / _rates100UnitsMainToDKK[moneyCurrency], DecimalPlaces);

            // Act
            var exchangedAmount = _theConverter.Convert(currencyPair, mainDkkAmount);

            // Assert
            exchangedAmount.ShouldBe(expectedExchangeAmount, DecimalPlaces);
        }

        private static IEnumerable<object[]> CombineAllNonDkkCurrencies()
        {
            var nonDkkCurrencies = new List<string>
            {
                EuroISO,
                UsDollarISO,
                BritishPoundISO,
                SwedishKronaISO,
                NorwegianKroneISO,
                SwissFrancISO,
                JapaneseYenISO
            };

            foreach (var mainCurrency in nonDkkCurrencies)
            {
                foreach (var moneyCurrency in nonDkkCurrencies)
                {
                    if (mainCurrency != moneyCurrency)
                    {
                        // Convert from Main to DKK, and then from DKK to Money currency
                        var inputAmount = AutoFixture.Create<decimal>();
                        var mainToDKKRate = _rates100UnitsMainToDKK[mainCurrency] / MainCurrencyVolume;
                        var dkkToMoneyRate = MainCurrencyVolume / _rates100UnitsMainToDKK[moneyCurrency];
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
        [InlineAutoData($"{EuroISO}/{EuroISO}")]
        [InlineAutoData($"{DanishKroneISO}/{DanishKroneISO}")]
        [InlineAutoData($"{SwedishKronaISO}/{SwedishKronaISO}")]
        [InlineAutoData($"{SwissFrancISO}/{SwissFrancISO}")]
        [InlineAutoData($"{NorwegianKroneISO}/{NorwegianKroneISO}")]
        [InlineAutoData($"{JapaneseYenISO}/{JapaneseYenISO}")]
        [InlineAutoData($"{UsDollarISO}/{UsDollarISO}")]
        [InlineAutoData($"{BritishPoundISO}/{BritishPoundISO}")]

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