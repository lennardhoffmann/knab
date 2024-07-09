using knab.ExternalCryptoDataProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knab.ExternalCryptoDataProvider.Services
{
    public interface IExternalCryptoProviderService
    {
        Task<Dictionary<string, ExternalCryptoDataProviderCryptoQuote>> GetExternalCryptoDataForCurrencyCodeAsync(string cryptoCurrencyCode);
    }
}
