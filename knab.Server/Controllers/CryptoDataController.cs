using knab.API.Services;
using knab.ExternalCryptoDataProvider.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace knab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoDataController : ControllerBase
    {
        private readonly IExternalCryptoProviderService _externalCryptoProviderService;
        private readonly ICryptoCurrencyDataService _cryptoDataService;
        public CryptoDataController(IExternalCryptoProviderService externalCryptoProviderService, ICryptoCurrencyDataService cryptoService)
        {
            _externalCryptoProviderService = externalCryptoProviderService;
            _cryptoDataService = cryptoService;
        }
        // GET: api/<CryptoData>
        [HttpGet]
        public async Task<IActionResult> GetCryptoProperties()
        {
            await _cryptoDataService.GetCryptoCurrencyProperties();
            return Ok();
        }

        [HttpGet("{cryptoCurrency}")]
        public async Task<IActionResult> GetExternalCryptoCurrencyData(string cryptoCurrency)
        {
            var result = await _externalCryptoProviderService.GetExternalCryptoDataForCurrencyCodeAsync(cryptoCurrency);
            await _cryptoDataService.StoreRequestForCryptoCurrency(cryptoCurrency, result);

            return Ok(result);
        }

    }
}
