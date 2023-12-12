namespace Domain
{
    public record ExchangeRate(
        string MainCurrency,
        string MoneyCurrency,
        int MainCurrencyVolume,
        decimal MoneyCurrencyValue)
    {
    }
}