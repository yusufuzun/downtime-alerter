using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Alerter.WebApp.Infrastructure.AlertJobDomain;
using Alerter.WebApp.Infrastructure.AlertJobDomain.Events;
using Alerter.WebApp.Infrastructure.Data;

namespace Alerter.WebApp.Infrastructure
{
    public class AlertJobService : IAlertJobService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMediator mediator;

        public AlertJobService(ApplicationDbContext applicationDbContext, IMediator mediator)
        {
            this.applicationDbContext = applicationDbContext;
            this.mediator = mediator;
        }

        public async Task<List<AlertJob>> GetAlertJobsAsync(Guid userId)
        {
            return await applicationDbContext.AlertJobs.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<AlertJob> GetAlertJobDetailsAsync(int id)
        {
            return await applicationDbContext.AlertJobs.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAlertJobAsync(Guid userId, AlertJob alertJob)
        {
            var alertJobDb = new AlertJob(userId, alertJob.Name, alertJob.Url, alertJob.IntervalInMinutes);
            applicationDbContext.Add(alertJobDb);

            await applicationDbContext.SaveChangesAsync();

            try
            {
                await mediator.Publish(new AlertJobCreatedEvent(alertJobDb.Id, alertJobDb.Url, alertJobDb.IntervalInMinutes));
            }
            catch
            {
                //undo changes
            }
        }

        public async Task DeleteAlertJobAsync(Guid userId, int id)
        {
            var alertJobDb = await GetAlertJobDetailsAsync(userId, id);

            applicationDbContext.AlertJobs.Remove(alertJobDb);

            await applicationDbContext.SaveChangesAsync();

            await mediator.Publish(new AlertJobDeletedEvent(alertJobDb.Id));
        }

        public async Task<AlertJob> GetAlertJobDetailsAsync(Guid userId, int id)
        {
            return await applicationDbContext.AlertJobs.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        }

        public async Task UpdateAlertJobAsync(Guid userId, AlertJob alertJob)
        {
            var alertJobDb = await GetAlertJobDetailsAsync(userId, alertJob.Id);
            if (alertJobDb == null)
            {
                return;
            }

            alertJobDb.Update(alertJob.Name, alertJob.Url, alertJob.IntervalInMinutes);

            applicationDbContext.Update(alertJobDb);
            await applicationDbContext.SaveChangesAsync();

            await mediator.Publish(new AlertJobUpdatedEvent(alertJobDb.Id, alertJobDb.Url, alertJobDb.IntervalInMinutes));
        }
    }
}
