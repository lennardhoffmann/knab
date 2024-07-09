using knab.DataAccess.Models;

namespace knab.DataAccess.Repositories
{
    public interface ICryptoDataProviderSettingsRepository
    {
        Task<ExternalCryptoProviderSettings> GetSettingsForDefaultProvider(string defaultProvider);
    }
}
