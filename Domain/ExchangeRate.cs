namespace Domain
{
    public record ExchangeRate(
        Currency MainCurrency,
        Currency MoneyCurrency,
        int MainCurrencyVolume,
        decimal MoneyCurrencyValue)
    {
    }
}