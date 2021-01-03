using Hangfire;
using MediatR;
using System.Threading.Tasks;
using Alerter.WebApp.AlertHttpClient;
using Alerter.WebApp.Data.Events;
using Hangfire.Common;

namespace Alerter.WebApp.JobManagement
{
    public class AlertJobSchedulingManager : IAlertJobSchedulingManager
    {
        private const string JobIdPrefix = "RC:Alert:";

        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IStatusCheckClient _statusCheckClient;
        private readonly IMediator _mediator;

        public AlertJobSchedulingManager(
            IRecurringJobManager recurringJobManager,
            IStatusCheckClient statusCheckClient,
            IMediator mediator)
        {
            _recurringJobManager = recurringJobManager;
            _statusCheckClient = statusCheckClient;
            _mediator = mediator;
        }

        public async Task AddOrUpdateAsync(int id, string url, int interval)
        {
            _recurringJobManager.AddOrUpdate(JobIdPrefix + id, Job.FromExpression(() => ScheduleAlertJob(id, url)), Cron.MinuteInterval(interval));
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(int id)
        {
            _recurringJobManager.RemoveIfExists(JobIdPrefix + id);
            await Task.CompletedTask;
        }

        public async Task ScheduleAlertJob(int id, string url)
        {
            var result = await _statusCheckClient.CheckStatus2XX(url);

            await _mediator.Publish(new StatusCheckedEvent(id, result));
        }
    }
}
