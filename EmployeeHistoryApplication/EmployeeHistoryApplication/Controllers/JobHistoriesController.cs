using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Data;
using EmployeeHistoryApplication.Models;
using Microsoft.Extensions.Localization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace EmployeeHistoryApplication.Controllers
{
    public class JobHistoriesController : Controller
    {
        private readonly EmployeeHistoryApplicationContext _context;
        private readonly IStringLocalizer<JobHistoriesController> _localizer;


        public JobHistoriesController(EmployeeHistoryApplicationContext context, IStringLocalizer<JobHistoriesController> localizer)
        {
            _localizer = localizer;
            _context = context;
        }

        public virtual JsonResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, JobHistory e)
        {
            
            
                _context.JobHistory.Remove(_context.JobHistory.Where(x => x.Id == e.Id).FirstOrDefault());
                _context.SaveChangesAsync();

            

            return Json(new[] { e }.ToDataSourceResult(request, ModelState));
        }

        // GET: JobHistories
        public async Task<IActionResult> Index()
        {
            var jobHistories = _context.JobHistory
                .Include(j => j.Employee)
                .OrderByDescending(j => j.dateFrom);
            return View(await jobHistories.ToListAsync());
        }

        // GET: JobHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var jobHistory = await _context.JobHistory
                .Include(j => j.Employee)
                .OrderByDescending(j => j.dateFrom)
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (jobHistory == null) return NotFound();

            return View(jobHistory);
        }

        // GET: JobHistories/Create
        public async Task<IActionResult> Create(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return NotFound();

            SetEmployeeViewData(employee);

            return View();
        }

        // POST: JobHistories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,CompanyName,JobPostition,dateFrom,dateTo")] JobHistory jobHistory)
        {
            jobHistory.Employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            if (jobHistory.Employee == null) return NotFound();

            // Validate the job history data
            if (!await ValidateJobHistoryDates(jobHistory))
            {
                SetEmployeeViewData(jobHistory.Employee);
                return View(jobHistory);
            }
            jobHistory.Employee.jobs.Add(jobHistory);
            _context.Update(jobHistory.Employee);
            _context.Add(jobHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
        }

        // GET: JobHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var jobHistory = await _context.JobHistory.FindAsync(id.Value);
            if (jobHistory == null) return NotFound();

            var employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            SetEmployeeViewData(employee);

            return View(jobHistory);
        }

        // POST: JobHistories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,CompanyName,JobPostition,dateFrom,dateTo")] JobHistory jobHistory)
        {
            if (id != jobHistory.Id) return NotFound();

            jobHistory.Employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            if (jobHistory.Employee == null) return NotFound();

            // Validate the job history data
            if (!await ValidateJobHistoryDates(jobHistory))
            {
                SetEmployeeViewData(jobHistory.Employee);
                return View(jobHistory);
            }
            jobHistory.Employee.jobs.Add(jobHistory);
            _context.Update(jobHistory.Employee);
            _context.Update(jobHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
        }

        // GET: JobHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var jobHistory = await _context.JobHistory
                .Include(j => j.Employee)
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (jobHistory == null) return NotFound();

            return View(jobHistory);
        }

        // POST: JobHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobHistory = await _context.JobHistory.FindAsync(id);
            if (jobHistory != null)
            {
                //jobHistory.Employee.jobs.Remove(jobHistory);
              //  _context.Update(jobHistory.Employee);
                _context.JobHistory.Remove(jobHistory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
        }

        private bool JobHistoryExists(int id) => _context.JobHistory.Any(e => e.Id == id);

        private void SetEmployeeViewData(Employee employee)
        {
            ViewData["EmployeeId"] = employee.Id;
            ViewData["EmployeeName"] = employee.Name;
            ViewData["EmployeeSurname"] = employee.Surname;
        }

        private async Task<bool> ValidateJobHistoryDates(JobHistory jobHistory)
        {
            if (!JobHistory.IsAfterCurrentDate(jobHistory.dateFrom))
            {
                ModelState.AddModelError("dateTo", "The starting date must not be after today's date.");
                return false;
            }

            DateTime? dateToToCompare = jobHistory.dateTo ?? DateTime.Now;

            var existingJobHistories = await _context.JobHistory
                .Where(j => j.EmployeeId == jobHistory.EmployeeId && j.Id != jobHistory.Id)
                .ToListAsync();

            if (jobHistory.dateTo == null && !JobHistory.IsThereACurrentDateNull(existingJobHistories))
            {
                ModelState.AddModelError("dateTo", "There is already a current job. Edit that one, then add a new current job.");
                return false;
            }

            if (jobHistory.dateTo == null && !JobHistory.IsDateRangeValidForNullDateTo(existingJobHistories, jobHistory.dateFrom, dateToToCompare))
            {
                ModelState.AddModelError("dateTo", "This is not the earliest job.");
                return false;
            }

            if (!JobHistory.IsDateRangeValid(existingJobHistories, jobHistory.dateFrom, dateToToCompare))
            {
                ModelState.AddModelError("dateTo", "The date range overlaps with an existing job history.");
                return false;
            }

            if (jobHistory.dateTo != null && jobHistory.dateFrom >= jobHistory.dateTo)
            {
                ModelState.AddModelError("dateTo", "The start date must be before the end date.");
                return false;
            }

            return true;
        }
    }
}
