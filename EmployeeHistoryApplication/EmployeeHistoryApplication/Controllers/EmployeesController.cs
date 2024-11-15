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

namespace EmployeeHistoryApplication.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeHistoryApplicationContext _context;

        public EmployeesController(EmployeeHistoryApplicationContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 2;

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

            return View(employees);
        }


        public async Task<IActionResult> Details(int? id, string sortOrder, int page = 1)
        {
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
            var jobs = await jobsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)jobsCount / pageSize);

            ViewData["Jobs"] = jobs;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SortOrder"] = sortOrder;

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
        public async Task<IActionResult> Edit(int? id, string sortOrder, int page = 1)
        {

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
            var jobs = await jobsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)jobsCount / pageSize);

            ViewData["Jobs"] = jobs;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SortOrder"] = sortOrder;

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

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
        public async Task<IActionResult> ExportToExcel()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");
            var employees = _context.Employee.ToList();
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Surname";
            worksheet.Cell(1, 3).Value = "Adress";
            worksheet.Cell(1, 4).Value = "EMBG";
            int i = 2;
            foreach (Employee employee in employees)
            {
                worksheet.Cell(i, 1).Value = employee.Name;
                worksheet.Cell(i, 2).Value = employee.Surname;
                worksheet.Cell(i, 3).Value = employee.Adress;
                worksheet.Cell(i, 4).Value = employee.EMBG;

                i++;

            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "example.xlsx");
            }
        }

    }
}
