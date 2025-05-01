using Microsoft.AspNetCore.Mvc;
using Test_API.Domains;
using Test_API.DTO;

namespace Test_API.Interfaces
{
    public interface IRequestAuditServices
    {
        public void LogWrite(string ResourceType , int Id );
    }
}