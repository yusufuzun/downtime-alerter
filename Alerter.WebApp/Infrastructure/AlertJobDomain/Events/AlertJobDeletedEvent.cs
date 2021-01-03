using MediatR;

namespace Alerter.WebApp.Infrastructure.AlertJobDomain.Events
{
    public class AlertJobDeletedEvent : INotification
    {
        public int Id { get; }

        public AlertJobDeletedEvent(int id)
        {
            this.Id = id;
        }
    }

}
