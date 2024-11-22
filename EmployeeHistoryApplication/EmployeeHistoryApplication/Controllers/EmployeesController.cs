using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeHistoryApplication.Data;
using EmployeeHistoryApplication.Models;
using Microsoft.Data.SqlClient;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace EmployeeHistoryApplication.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeHistoryApplicationContext _context;
        private readonly IStringLocalizer<EmployeesController> _localizer;


        public EmployeesController(EmployeeHistoryApplicationContext context, IStringLocalizer<EmployeesController> localizer)
        {
            _localizer = localizer;
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString, int page = 1)
        {

            int pageSize = 2;
            var teskt = _localizer["Asc"];
            var employeesQuery = _context.Employee.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                employeesQuery = employeesQuery.Where(e => e.Name.ToLower().Contains(searchString)
                                                       || e.Surname.ToLower().Contains(searchString));
            }


            int totalEmployees = await employeesQuery.CountAsync();


            var employees = await employeesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var totalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);


            ViewData["SearchString"] = searchString;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            ViewData["TotalEmployees"] = totalEmployees;
            ViewData["DisplayedEmployees"] = employees.Count;

            return View(employees);
        }

        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employees = _context.Employee
                .Select(e => new Employee
                {
                    Id = e.Id,
                    Name = e.Name,
                    Surname = e.Surname,
                    Adress = e.Adress,
                    EMBG = e.EMBG
                })
                .ToList();

            return Json(employees.ToDataSourceResult(request));
        }


        //ovde da se sortira listata pa da se prikazi
        public async Task<IActionResult> Details(int? id, string sortOrder, int page = 1)
        {

            var text = _localizer["EditString"];
            var currentCulture = CultureInfo.CurrentCulture.Name;
            int pageSize = 2;

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var jobsQuery = _context.JobHistory
                .Where(j => j.EmployeeId == id);


            int jobsCount = await jobsQuery.CountAsync();
            var jobs = await jobsQuery.ToListAsync();
            //tuka sortiraj
            switch (sortOrder)
            {
                case "dateFrom_desc":
                    jobs = jobs.OrderByDescending(j => j.dateFrom).ToList();
                    break;
                case "dateTo_desc":
                    jobs = jobs.OrderByDescending(j => j.dateTo).ToList();
                    break;
                case "dateFrom":
                    jobs = jobs.OrderBy(j => j.dateFrom).ToList();
                    break;
                case "dateTo":
                    jobs = jobs.OrderBy(j => j.dateTo).ToList();
                    break;
                default:
                    jobs = jobs.OrderByDescending(j => j.dateFrom).ToList();
                    break;
            }

            jobs = jobs
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();

            var totalPages = (int)Math.Ceiling((double)jobsCount / pageSize);

            ViewData["Jobs"] = jobs;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SortOrder"] = sortOrder;
            ViewData["TotalJobs"] = jobsCount;
            ViewData["DisplayedJobs"] = jobs.Count;


            return View(employee);
        }



        public JsonResult Grid_Read_Jobs([DataSourceRequest] DataSourceRequest request, int id)
        {
            var jobsQuery = _context.JobHistory
             .Where(j => j.EmployeeId == id);

            return Json(jobsQuery.ToDataSourceResult(request));
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


        public async Task<IActionResult> Edit(int? id, string sortOrder, int page = 1)
        {

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            var jobsQuery = _context.JobHistory
              .Where(j => j.EmployeeId == id);


            var jobs = await jobsQuery.ToListAsync();
            ViewData["Jobs"] = jobs;
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

            var jobsQuery = _context.JobHistory
             .Where(j => j.EmployeeId == id);

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

        public virtual JsonResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, Employee e)
        {
            if (ModelState.IsValid)
            {
                _context.Employee.Remove(_context.Employee.Where(x=>x.Id==e.Id).FirstOrDefault());
                _context.SaveChangesAsync();

            }

            return Json(new[] { e }.ToDataSourceResult(request, ModelState));
        }



        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
       
       
    }
}
