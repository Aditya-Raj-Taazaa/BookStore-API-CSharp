using Microsoft.Extensions.Configuration;

namespace Test_API.Services
{
    public class AppInfoService
    {
        private readonly IConfiguration _config;

        public AppInfoService(IConfiguration config)
        {
            _config = config;
        }

        public string GetAppName() => _config["Build:App_Name"] ?? "Unknown App";
        public string GetVersion() => _config["Build:Version"] ?? "Unknown Version";
    }
}