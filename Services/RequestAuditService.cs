using Microsoft.Extensions.Logging;

namespace Test_API.Services
{
    public class RequestAuditService
    {
        private readonly ILogger<RequestAuditService> _logger;

        public RequestAuditService(ILogger<RequestAuditService> logger)
        {
            _logger = logger;
        }

        public void LogWrite(string ResourceType , int Id )
        {
            Console.WriteLine($" 🔓 accessed {ResourceType} with ID : {Id}");
        }
    }
}
