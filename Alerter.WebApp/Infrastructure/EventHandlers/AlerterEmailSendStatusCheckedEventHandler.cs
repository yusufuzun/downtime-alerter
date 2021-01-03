using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Alerter.WebApp.Data.Events;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Alerter.WebApp.Infrastructure.AlertNotifying;

namespace Alerter.WebApp.Infrastructure.EventHandlers
{
    public class AlerterEmailSendStatusCheckedEventHandler : INotificationHandler<StatusCheckedEvent>
    {
        private readonly IAlertJobService alertJobService;
        private readonly IAlertNotifierService alertNotifierService;
        private readonly UserManager<IdentityUser> userManager;

        public AlerterEmailSendStatusCheckedEventHandler(IAlertJobService alertJobService, IAlertNotifierService alertNotifierService, UserManager<IdentityUser> userManager)
        {
            this.alertJobService = alertJobService;
            this.alertNotifierService = alertNotifierService;
            this.userManager = userManager;
        }

        public async Task Handle(StatusCheckedEvent notification, CancellationToken cancellationToken)
        {
            var alert = await alertJobService.GetAlertJobDetailsAsync(notification.Id);
            if (alert == null)
            {
                return;
            }

            var user = await userManager.FindByIdAsync(alert.UserId.ToString());

            await alertNotifierService.SendNotificationAsync(new Dictionary<string, string> {
                {"email", user.Email },
                {"subject", "Alerter Notification" },
                {"message", $"Error in #{alert.Id} job" }
            });
        }
    }
}
