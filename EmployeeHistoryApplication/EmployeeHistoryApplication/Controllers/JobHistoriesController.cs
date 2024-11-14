using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Data;
using EmployeeHistoryApplication.Models;

namespace EmployeeHistoryApplication.Controllers
{
    public class JobHistoriesController : Controller
    {
        private readonly EmployeeHistoryApplicationContext _context;

        public JobHistoriesController(EmployeeHistoryApplicationContext context)
        {
            _context = context;
        }

        // GET: JobHistories
        public async Task<IActionResult> Index()
        {
            var employeeHistoryApplicationContext = _context.JobHistory
       .Include(j => j.Employee)
       .OrderByDescending(j => j.dateFrom);
            return View(await employeeHistoryApplicationContext.ToListAsync());
        }

        // GET: JobHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobHistory = await _context.JobHistory
                .Include(j => j.Employee).OrderByDescending(j=>j.dateFrom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobHistory == null)
            {
                return NotFound();
            }

            return View(jobHistory);
        }

        // GET: JobHistories/Create
        public async Task<IActionResult> Create(int id)
        {
            Employee employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = id;
            ViewData["EmployeeName"] = employee.Name;
            ViewData["EmployeeSurname"] = employee.Surname;

            return View();
        }

        // POST: JobHistories/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,CompanyName,JobPostition,dateFrom,dateTo")] JobHistory jobHistory)
        {
            jobHistory.Employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            if (jobHistory.Employee == null)
            {
                ModelState.AddModelError("EmployeeId", "Invalid Employee ID");
                ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name", jobHistory.EmployeeId);
                return RedirectToAction("Index", "Employees");
            }
            else
            {
                DateTime? dateToToCompare = null;
                if (jobHistory.dateTo==null)
                {
                    dateToToCompare = DateTime.Now;
                }
                else
                {
                    dateToToCompare= jobHistory.dateTo;
                }
                var existingJobHistories = _context.JobHistory.Where(j => j.EmployeeId == jobHistory.EmployeeId && j.Id != jobHistory.Id)
                    .ToList();
                if (!JobHistory.IsDateRangeValid(existingJobHistories, jobHistory.dateFrom, dateToToCompare))
                {
                    ModelState.AddModelError("dateTo", "\"The date range overlaps with an existing job history.");
                    ViewData["EmployeeId"] = jobHistory.EmployeeId;
                    ViewData["EmployeeName"] = jobHistory.Employee.Name;
                    ViewData["EmployeeSurname"] = jobHistory.Employee.Surname;
                    return View(jobHistory);
                }
                else if (jobHistory.dateTo!=null&&jobHistory.dateFrom >= jobHistory.dateTo)
                {
                    ModelState.AddModelError("dateTo", "The start date must be before the end date.");
                    ViewData["EmployeeId"] = jobHistory.EmployeeId;
                    ViewData["EmployeeName"] = jobHistory.Employee.Name;
                    ViewData["EmployeeSurname"] = jobHistory.Employee.Surname;
                    return View(jobHistory);
                }
                else
                {
                    _context.Add(jobHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
                }
            }
            ViewData["EmployeeId"] = jobHistory.EmployeeId;
            ViewData["EmployeeName"] = jobHistory.Employee.Name;
            ViewData["EmployeeSurname"] = jobHistory.Employee.Surname;
            return View(jobHistory);
        }

        // GET: JobHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobHistory = await _context.JobHistory.FindAsync(id);
            if (jobHistory == null)
            {
                return NotFound();
            }
            Employee employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            ViewData["EmployeeId"] = employee.Id;
            ViewData["EmployeeName"] = employee.Name;
            ViewData["EmployeeSurname"] = employee.Surname;
            return View(jobHistory);
        }

        // POST: JobHistories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,CompanyName,JobPostition,dateFrom,dateTo")] JobHistory jobHistory)
        {
            if (id != jobHistory.Id)
            {
                return NotFound();
            }
            jobHistory.Employee = await _context.Employee.FindAsync(jobHistory.EmployeeId);
            if (jobHistory.Employee == null)
            {
                return NotFound();
            }
            else
            {
                DateTime? dateToToCompare = null;
                if (jobHistory.dateTo == null)
                {
                    dateToToCompare = DateTime.Now;
                }
                else
                {
                    dateToToCompare = jobHistory.dateTo;
                }
                var existingJobHistories = _context.JobHistory.Where(j => j.EmployeeId == jobHistory.EmployeeId && j.Id != jobHistory.Id)
      .ToList();
                if (!JobHistory.IsDateRangeValid(existingJobHistories, jobHistory.dateFrom, dateToToCompare))
                {
                    ModelState.AddModelError("dateTo", "\"The date range overlaps with an existing job history.");
                    ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Id", jobHistory.EmployeeId);
                    return View(jobHistory);
                }
                else if (jobHistory.dateTo != null&&jobHistory.dateFrom >= jobHistory.dateTo)
                {
                    ModelState.AddModelError("dateTo", "The start date must be before the end date.");
                    ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Id", jobHistory.EmployeeId);
                    return View(jobHistory);
                }
                else
                {
                    _context.Update(jobHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
                }
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Id", jobHistory.EmployeeId);
            return View(jobHistory);
        }

        // GET: JobHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobHistory = await _context.JobHistory
                .Include(j => j.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobHistory == null)
            {
                return NotFound();
            }

            return View(jobHistory);
        }

        // POST: JobHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobHistory = await _context.JobHistory.FindAsync(id);
            if (jobHistory != null)
            {
                _context.JobHistory.Remove(jobHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
        }

        private bool JobHistoryExists(int id)
        {
            return _context.JobHistory.Any(e => e.Id == id);
        }
    }
}
