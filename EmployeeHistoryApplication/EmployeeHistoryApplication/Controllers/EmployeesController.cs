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
    public class EmployeesController : Controller
    {
        private readonly EmployeeHistoryApplicationContext _context;

        public EmployeesController(EmployeeHistoryApplicationContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = await _context.Employee.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                               || s.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

             ViewData["SearchString"] = searchString;
            return View(employees);
        }

        //ovde da se sortira po daden parametar
        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id, string sortOrder)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the employee with jobs
            var employee = await _context.Employee
                .Include(e => e.jobs) // Ensure jobs are included in the result
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Sort jobs based on the selected sort order
            ViewData["SortOrder"] = sortOrder;
            switch (sortOrder)
            {
                case "dateFrom_desc":
                    employee.jobs.ToList().Sort(new JobHistoryDateComparerDescending());
                    break;
                case "dateTo_desc":
                     employee.jobs.OrderByDescending(j => j.dateTo).ToList();
                    break;
                case "dateFrom":
                     employee.jobs.OrderBy(j => j.dateFrom).ToList();
                    break;
                case "dateTo":
                    employee.jobs.OrderBy(j => j.dateTo).ToList();
                    break;
                default:
                    // Default sorting, maybe by job title or by start date
                    employee.jobs.OrderByDescending(j => j.dateFrom).ToList();
                    break;
                    
            }

            // Pass the employee model (including sorted jobs) to the view
            return View(employee);
        }




        // GET: Employees/Create
        public IActionResult Create()
        {
            return View(); // otvora create view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Adress,EMBG")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        //Ovde da se izvrsi sortiranje po date to desc
        //ovde da se sortira po daden parametar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                           .Include(e => e.jobs) //treba da se zemat i jobs
                           .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Adress,EMBG")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
