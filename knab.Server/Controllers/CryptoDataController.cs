using knab.DataAccess.Services;
using knab.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace knab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CryptoDataController : ControllerBase
    {
        private readonly IExternalCryptoProviderService _externalCryptoProviderService;
        private readonly ICryptoCurrencyDataService _cryptoDataService;

        /// <summary>
        /// Controller for retrieveing relevant data pertaining to crypto currencies.
        /// </summary>
        public CryptoDataController(IExternalCryptoProviderService externalCryptoProviderService, ICryptoCurrencyDataService cryptoService)
        {
            _externalCryptoProviderService = externalCryptoProviderService;
            _cryptoDataService = cryptoService;
        }

        /// <summary>
        /// Retrieves all crypto currency properties from the database.
        /// </summary>
        [HttpGet("getCryptoProperties")]
        public async Task<IActionResult> GetCryptoProperties()
        {
            var result = await _cryptoDataService.GetCryptoCurrencyProperties();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves exchange rate data for a specific crypto currency from an external data provider.
        /// </summary>
        /// <param name="cryptoCurrency">The currency for which data has to be retrieved. E.g BTC</param>
        [HttpGet("{cryptoCurrency}")]
        public async Task<IActionResult> GetExternalCryptoCurrencyData(string cryptoCurrency)
        {
            var result = await _externalCryptoProviderService.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrency.ToUpper());
            await _cryptoDataService.StoreRequestForCryptoCurrency(cryptoCurrency.ToUpper(), result);

            return Ok(result);
        }

    }
}
