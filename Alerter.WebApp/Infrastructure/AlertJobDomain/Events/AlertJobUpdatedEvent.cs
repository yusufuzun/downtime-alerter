using MediatR;

namespace Alerter.WebApp.Infrastructure.AlertJobDomain.Events
{
    public class AlertJobUpdatedEvent : INotification
    {
        public int Id { get; }
        public string Url { get; }
        public int IntervalInMinutes { get; }

        public AlertJobUpdatedEvent(int id, string url, int intervalInMinutes)
        {
            Id = id;
            Url = url;
            IntervalInMinutes = intervalInMinutes;
        }
    }

}
