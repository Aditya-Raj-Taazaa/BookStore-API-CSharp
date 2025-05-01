using Microsoft.Extensions.Configuration;

namespace Test_API.Interfaces
{
    public interface IAppInfoService
    {
        string GetAppName();
        string GetVersion();
    }
}