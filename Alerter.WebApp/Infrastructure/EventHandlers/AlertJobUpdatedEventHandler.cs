using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Alerter.WebApp.Infrastructure.AlertJobDomain.Events;
using Alerter.WebApp.JobManagement;

namespace Alerter.WebApp.Infrastructure.EventHandlers
{
    public class AlertJobUpdatedEventHandler : INotificationHandler<AlertJobUpdatedEvent>
    {
        private readonly IAlertJobSchedulingManager alertJobSchedulingManager;

        public AlertJobUpdatedEventHandler(IAlertJobSchedulingManager alertJobSchedulingManager)
        {
            this.alertJobSchedulingManager = alertJobSchedulingManager;
        }

        public async Task Handle(AlertJobUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await alertJobSchedulingManager.AddOrUpdateAsync(notification.Id, notification.Url, notification.IntervalInMinutes);
        }
    }
}
