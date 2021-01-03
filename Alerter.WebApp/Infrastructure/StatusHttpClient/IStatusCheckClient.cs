using System.Threading.Tasks;

namespace Alerter.WebApp.AlertHttpClient
{
    public interface IStatusCheckClient
    {
        Task<bool> CheckStatus2XX(string url);
    }
}
