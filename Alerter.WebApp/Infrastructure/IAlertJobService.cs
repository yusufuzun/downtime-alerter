using Alerter.WebApp.Infrastructure.AlertJobDomain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alerter.WebApp.Infrastructure
{
    public interface IAlertJobService
    {
        Task CreateAlertJobAsync(Guid userId, AlertJob alertJob);
        Task DeleteAlertJobAsync(Guid userId, int id);
        Task<AlertJob> GetAlertJobDetailsAsync(Guid userId, int id);
        Task<AlertJob> GetAlertJobDetailsAsync(int id);
        Task<List<AlertJob>> GetAlertJobsAsync(Guid userId);
        Task UpdateAlertJobAsync(Guid userId, AlertJob alertJob);
    }
}