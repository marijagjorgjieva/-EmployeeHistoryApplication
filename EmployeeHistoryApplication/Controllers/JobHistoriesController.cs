using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Data;
using EmployeeHistoryApplication.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var employeeHistoryApplicationContext = _context.JobHistory.Include(j => j.Employee);
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
                .Include(j => j.Employee)
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
              /*  //pred da se addne da se proverat datumite
                if (ModelState.IsValid)
                {*/
                    _context.Add(jobHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
              /*  }
                else
                {
                    ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name", jobHistory.EmployeeId);
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
                }*/
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name", jobHistory.EmployeeId);
            return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
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
            ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Id", jobHistory.EmployeeId);
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
                ModelState.AddModelError("EmployeeId", "Invalid Employee ID");
                ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Name", jobHistory.EmployeeId);
                return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
            }
            else
            {
                var existingJobHistories = _context.JobHistory.Where(j => j.EmployeeId == jobHistory.EmployeeId && j.Id != jobHistory.Id)
       .ToList();
                if (!JobHistory.IsDateRangeValid(existingJobHistories, jobHistory.dateFrom, jobHistory.dateTo))
                {
                    ModelState.AddModelError("", "The date range overlaps with an existing job history.");
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
                }
                //pred da se update da se provere datumot  
                if (ModelState.IsValid)
                {
                    _context.Update(jobHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", "Employees", new { id = jobHistory.EmployeeId });
                }
                else
                {
                    ViewData["EmployeeId"] = new SelectList(_context.Employee, "Id", "Id", jobHistory.EmployeeId);
                    return View(jobHistory);
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


