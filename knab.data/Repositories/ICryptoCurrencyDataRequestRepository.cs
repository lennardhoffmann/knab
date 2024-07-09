using knab.DataAccess.Models;

namespace knab.DataAccess.Repositories
{
    public interface ICryptoCurrencyDataRequestRepository
    {
        Task<CryptoCurrencyDataRequest?> GetDataRequestRecordByCryptoCurrencyCodeAsync(string cryptoCurrencyCode);
        Task AddDataRequestForCryptoCurrencyAsync(CryptoCurrencyDataRequest requestObject);
        Task UpdateDataRequestForCryptoCurrencyAsync(CryptoCurrencyDataRequest requestObject);
    }
}
