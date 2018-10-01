using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;
using System.Data.SqlClient;

/* 
    AUTHORS: Elliot Huck, April Watson
    PURPOSE: To prescribe available actions to the user pertaining to viewing Departments.
*/

namespace WorkforceManagement.Controllers
{
	public class DepartmentController : Controller
	{
		private readonly IConfiguration _config;

		public DepartmentController(IConfiguration config)
		{
			_config = config;
		}

		public IDbConnection Connection
		{
			get
			{
				return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			}
		}

		// GET: Department
		// This GET method returns all the departments and provides them to View/Department/Index.cshtml as an IEnumerable<Department>

        public async Task<IActionResult> Index()
        {
            string sql = @"
			SELECT * FROM Departments
			ORDER BY Name
			";
            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> allDepartments = await conn.QueryAsync<Department>(sql);
                return View(allDepartments);
            }
        }

        // /Department/Create will send the user to a form to create a new department
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // On submit of the new department the Create Post will check to see if the department name already exists
        // if it does not exist the department will be posted to BangazonAPI DB. If department already exists
        // the user will stay on the create new form view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {

                string sql = $@"INSERT INTO Departments
                            (Name, Budget)
                            VALUES
                            ('{department.Name}', {department.Budget});";

                if (CheckDepartmentDoesNotExist(department.Name))
                {
                    using (IDbConnection conn = Connection)
                    {
                        int addDepartment = await conn.ExecuteAsync(sql);
                        if (addDepartment == 1)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            //return RedirectToAction(nameof(CannotCreate(department)));
            return View(department);
        }

        //public IActionResult CannotCreate(Department department)
        //{
        //    return View(department);
        //}

        private bool CheckDepartmentDoesNotExist(string name)
        {
            string sql = $"SELECT * FROM Departments d WHERE d.Name = '{name}';";

            using (IDbConnection conn = Connection)
            {
                var theCount = conn.Query<Department>(sql).Count();
                return theCount == 0;
            }
        }

        // Queries database for all employees by the specified department id.
        // Adds employees to the department's employee list and returns the department for display to the browser.
        public async Task<IActionResult> Details([FromRoute]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            select 
                d.Id,
                d.Name,
                d.Budget,
                e.Id,
                e.FirstName,
                e.LastName,
                e.HireDate,
                e.IsSupervisor,
                e.DepartmentId       
            From Departments d
            Join Employees e on d.Id = e.DepartmentId
            Where d.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {

                Department dept = new Department();
                var deptQuery = await conn.QueryAsync<Department, Employee, Department>(sql, (department, employee) =>
                {

                    dept.Id = department.Id;
                    dept.Name = department.Name;


                    dept.EmployeeList.Add(employee);
                    return department;
                }

                );

                return View(dept);
            }
        }
    }
}
