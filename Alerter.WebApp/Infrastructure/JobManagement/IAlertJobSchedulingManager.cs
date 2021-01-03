using System.Threading.Tasks;

namespace Alerter.WebApp.JobManagement
{
    public interface IAlertJobSchedulingManager
    {
        Task AddOrUpdateAsync(int id, string url, int interval);
        Task RemoveAsync(int id);
    }
}