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

        [HttpGet]
        public async Task<IActionResult> Create ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (Department department)
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
            //return RedirectToAction(nameof(CannotCreate(department)));
            return View(department);
        }

        //public IActionResult CannotCreate(Department department)
        //{
        //    return View(department);
        //}

        private bool CheckDepartmentDoesNotExist (string name)
        {
            string sql = $"SELECT * FROM Departments d WHERE d.Name = '{name}';";

            using (IDbConnection conn = Connection)
            {
                var theCount = conn.Query<Department>(sql).Count();
                return theCount == 0;
            }
        }
	}
}