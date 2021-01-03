using System;
using System.ComponentModel.DataAnnotations;

namespace Alerter.WebApp.Infrastructure.AlertJobDomain
{
    public class AlertJob
    {
        public AlertJob()
        {

        }

        public AlertJob(Guid userId, string name, string url, int intervalInMinutes)
        {
            UserId = userId;
            Name = name;
            Url = url;
            IntervalInMinutes = intervalInMinutes;
            CreatedDate = DateTimeOffset.Now;
        }

        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        public int IntervalInMinutes { get; set; }

        public string CurrentStatus { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public void SetStatus(bool status)
        {
            CurrentStatus = status ? "OK" : "NOK";
            LastUpdatedDate = DateTimeOffset.Now;
        }

        public void Update(string name, string url, int intervalInMinutes)
        {
            Name = name;
            Url = url;
            IntervalInMinutes = intervalInMinutes;
            LastUpdatedDate = DateTimeOffset.Now;
        }
    }
}
