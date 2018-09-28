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

namespace WorkforceManagement.Controllers
{
    /* 
		AUTHORS: Elliot Huck, April Watson
		PURPOSE: To prescribe available actions to the user pertaining to viewing Departments.
	*/
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