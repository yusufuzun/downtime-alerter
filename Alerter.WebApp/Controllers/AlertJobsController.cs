using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alerter.WebApp.Infrastructure;
using Alerter.WebApp.Infrastructure.AlertJobDomain;

namespace Alerter.WebApp.Controllers
{
    [Authorize]
    public class AlertJobsController : Controller
    {
        private readonly IAlertJobService alertJobService;

        public AlertJobsController(IAlertJobService alertJobService)
        {
            this.alertJobService = alertJobService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await alertJobService.GetAlertJobsAsync(userId);
            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await alertJobService.GetAlertJobDetailsAsync(userId, id.Value);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Url,IntervalInMinutes")] AlertJob alertJob)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await alertJobService.CreateAlertJobAsync(userId, alertJob);

                return RedirectToAction(nameof(Index));
            }
            return View(alertJob);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var alertJob = await alertJobService.GetAlertJobDetailsAsync(userId, id.Value);

            if (alertJob == null)
            {
                return NotFound();
            }
            return View(alertJob);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,IntervalInMinutes")] AlertJob alertJob)
        {
            if (id != alertJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                try
                {
                    await alertJobService.UpdateAlertJobAsync(userId, alertJob);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (alertJobService.GetAlertJobDetailsAsync(userId, id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(alertJob);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var alertJob = await alertJobService.GetAlertJobDetailsAsync(userId, id.Value);

            if (alertJob == null)
            {
                return NotFound();
            }

            return View(alertJob);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await alertJobService.DeleteAlertJobAsync(userId, id);

            return RedirectToAction(nameof(Index));
        }
    }

}
