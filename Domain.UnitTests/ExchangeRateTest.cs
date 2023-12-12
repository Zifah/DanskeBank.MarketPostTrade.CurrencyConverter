using Shouldly;
using Xunit;

namespace Domain.UnitTests
{
    public class ExchangeRateTest
    {
        [Fact]
        public void Invert_FlipsTheCurrenciesAndExchangeRate()
        {
            // Arrange
            string mainCurrency = "ABC", moneyCurrency = "DEF";
            int mainCurrencyVolume = 1;
            decimal moneyCurrencyValue = 5.08m;
            var exchangeRate = new ExchangeRate(mainCurrency, moneyCurrency, mainCurrencyVolume, moneyCurrencyValue);

            // Act
            var invertedRate = exchangeRate.Invert();

            // Assert
            invertedRate.ShouldBe(new ExchangeRate(moneyCurrency, mainCurrency, 1, mainCurrencyVolume/moneyCurrencyValue));
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 7.05)]
        [InlineData(100, 1)]
        public void GetSingleUnitRate_ReturnsTheCorrectRateForOneUnitOfMainCurrency(int mainVolume, decimal moneyValue)
        {
            // Arrange
            var exchangeRate = new ExchangeRate("ABC", "DEF", mainVolume, moneyValue);

            // Act
            var singleUnitValue = exchangeRate.GetSingleUnitRate();

            // Assert
            singleUnitValue.ShouldBe(moneyValue/mainVolume);
        }
    }
}