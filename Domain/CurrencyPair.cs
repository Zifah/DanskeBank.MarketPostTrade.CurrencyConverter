using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public record CurrencyPair(string MainCurrency, string MoneyCurrency)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyPair">Format: "MainCurrencyISOCode/MoneyCurrencyISOCode"</param>
        /// <returns></returns>
        public static CurrencyPair Build(string currencyPair)
        {
            var currencies = currencyPair.ToUpper().Split('/');

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
    }
}
