using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alerter.WebApp.Infrastructure.AlertNotifying
{
    public interface IAlertNotifierService
    {
        Task SendNotificationAsync(Dictionary<string, string> keyValuePairs);
    }
}
