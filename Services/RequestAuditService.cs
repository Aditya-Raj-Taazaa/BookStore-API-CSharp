using Test_API.Interfaces;
namespace Test_API.Services
{
    public class RequestAuditService : IRequestAuditServices
    {

        public RequestAuditService()
        {
        }

        public void LogWrite(string ResourceType , int Id )
        {
            Console.WriteLine($" 🔓 accessed {ResourceType} with ID : {Id}");
        }
    }
}
