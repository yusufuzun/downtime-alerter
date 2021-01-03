using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Alerter.WebApp.Infrastructure.AlertJobDomain.Events;
using Alerter.WebApp.JobManagement;

namespace Alerter.WebApp.Infrastructure.EventHandlers
{
    public class AlertJobDeletedEventHandler : INotificationHandler<AlertJobDeletedEvent>
    {
        private readonly IAlertJobSchedulingManager alertJobSchedulingManager;

        public AlertJobDeletedEventHandler(IAlertJobSchedulingManager alertJobSchedulingManager)
        {
            this.alertJobSchedulingManager = alertJobSchedulingManager;
        }

        public async Task Handle(AlertJobDeletedEvent notification, CancellationToken cancellationToken)
        {
            await alertJobSchedulingManager.RemoveAsync(notification.Id);
        }
    }
}
