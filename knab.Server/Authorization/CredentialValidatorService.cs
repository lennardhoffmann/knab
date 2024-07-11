using knab.API.Authorization.Models;

namespace knab.API.Authorization
{
    public class CredentialValidatorService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CredentialValidatorService> _logger;
        public CredentialValidatorService(IConfiguration configuration, ILogger<CredentialValidatorService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ValidateCredentials(AuthRequest credentials)
        {
            try
            {
                if (!IsValidCredentials(credentials))
                {
                    throw new Exception("Invalid credentials provided");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid credentials provided");
                throw new BadHttpRequestException(ex.Message);
            }
        }

        private bool IsValidCredentials(AuthRequest credentials)
        {
            var validDefaultCredentials = _configuration.GetSection("KnabCredentials").Get<AuthRequest>();
            if (string.IsNullOrEmpty(credentials.Username) || credentials.Username != validDefaultCredentials.Username)
            {
                return false;
            }

            if (string.IsNullOrEmpty(credentials.Password) || credentials.Password != validDefaultCredentials.Password)
            {
                return false;
            }

            return true;
        }
    }
}
