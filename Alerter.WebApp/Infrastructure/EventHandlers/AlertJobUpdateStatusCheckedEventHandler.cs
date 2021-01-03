using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;
using Alerter.WebApp.Data.Events;
using Alerter.WebApp.Hubs;

namespace Alerter.WebApp.Infrastructure.EventHandlers
{
    public class AlertJobUpdateStatusCheckedEventHandler : INotificationHandler<StatusCheckedEvent>
    {
        private readonly IAlertJobService alertJobService;
        private readonly IHubContext<AlertHub> hubContext;

        public AlertJobUpdateStatusCheckedEventHandler(IAlertJobService alertJobService, IHubContext<AlertHub> hubContext)
        {
            this.alertJobService = alertJobService;
            this.hubContext = hubContext;
        }

        public async Task Handle(StatusCheckedEvent notification, CancellationToken cancellationToken)
        {
            var alert = await alertJobService.GetAlertJobDetailsAsync(notification.Id);
            if (alert == null)
            {
                return;
            }

            alert.SetStatus(notification.Status);

            await alertJobService.UpdateAlertJobAsync(alert.UserId, alert);

            await hubContext.Clients.User(alert.UserId.ToString())
                .SendAsync("ReceiveStatus", alert.Id.ToString(), alert.CurrentStatus, alert.LastUpdatedDate.ToString(), cancellationToken);
        }
    }
}
