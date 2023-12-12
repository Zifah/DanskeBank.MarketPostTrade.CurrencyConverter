namespace Domain
{
    public record CurrencyPair
    {
        public string MainCurrency { get; }
        public string MoneyCurrency { get; }

        public CurrencyPair(string mainCurrency, string moneyCurrency)
        {
            MainCurrency = mainCurrency.Trim().ToUpper();
            MoneyCurrency = moneyCurrency.Trim().ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyPair">Format: "MainCurrencyISOCode/MoneyCurrencyISOCode"</param>
        /// <returns></returns>
        public static CurrencyPair Build(string currencyPair)
        {
            var currencies = currencyPair.Split('/');

            if (currencies.Length != 2)
            {
                throw new ArgumentException("A valid currency pair must contain two forward-slash separated currency ISO Codes.");
            }

            return new CurrencyPair(currencies[0], currencies[1]);
        }

        public override string ToString()
        {
            return $"{MainCurrency}/{MoneyCurrency}";
        }

        public bool AreSame()
        {
            return MainCurrency.Equals(MoneyCurrency);
        }
    }
}
