namespace Domain
{
    public record ExchangeRate(
        string MainCurrency,
        string MoneyCurrency,
        int MainCurrencyVolume,
        decimal MoneyCurrencyValue)
    {
        public ExchangeRate Invert()
        {
            return new ExchangeRate(MoneyCurrency, MainCurrency, 1, MainCurrencyVolume / MoneyCurrencyValue);
        }

        public decimal GetSingleUnitRate()
        {
            return MoneyCurrencyValue / MainCurrencyVolume;
        }
    }
}