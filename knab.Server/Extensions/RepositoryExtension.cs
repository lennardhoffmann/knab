using knab.DataAccess.Models;
using knab.DataAccess.Repositories;

namespace knab.API.Extensions
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<ICryptoCurrencyPropertyRepository, CryptoCurrencyPropertyRepository>();
            services.AddScoped<ICryptoCurrencyDataRequestRepository, CryptoCurrencyDataRequestRepository>();
            services.AddScoped<ICryptoDataProviderSettingsRepository, CryptoDataProviderSettingsRepository>();

            return services;
        }
    }
}
