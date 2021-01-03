using MediatR;

namespace Alerter.WebApp.Data.Events
{
    public class StatusCheckedEvent : INotification
    {
        public StatusCheckedEvent(int id, bool status)
        {
            Id = id;
            Status = status;
        }

        public int Id { get; internal set; }

        public bool Status { get; set; }
    }
}
