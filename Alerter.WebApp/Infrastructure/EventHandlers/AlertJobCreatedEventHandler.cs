using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Alerter.WebApp.Infrastructure.AlertJobDomain.Events;
using Alerter.WebApp.JobManagement;

namespace Alerter.WebApp.Infrastructure.EventHandlers
{
    public class AlertJobCreatedEventHandler : INotificationHandler<AlertJobCreatedEvent>
    {
        private readonly IAlertJobSchedulingManager alertJobSchedulingManager;

        public AlertJobCreatedEventHandler(IAlertJobSchedulingManager alertJobSchedulingManager)
        {
            this.alertJobSchedulingManager = alertJobSchedulingManager;
        }

        public async Task Handle(AlertJobCreatedEvent notification, CancellationToken cancellationToken)
        {
            await alertJobSchedulingManager.AddOrUpdateAsync(notification.Id, notification.Url, notification.IntervalInMinutes);
        }
    }
}
