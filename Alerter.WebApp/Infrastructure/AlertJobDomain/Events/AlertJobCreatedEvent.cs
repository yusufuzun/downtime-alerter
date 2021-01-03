using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alerter.WebApp.Infrastructure.AlertJobDomain.Events
{
    public class AlertJobCreatedEvent : INotification
    {
        public int Id { get; }
        public string Url { get; }
        public int IntervalInMinutes { get; }

        public AlertJobCreatedEvent(int id, string url, int intervalInMinutes)
        {
            Id = id;
            Url = url;
            IntervalInMinutes = intervalInMinutes;
        }
    }

}
