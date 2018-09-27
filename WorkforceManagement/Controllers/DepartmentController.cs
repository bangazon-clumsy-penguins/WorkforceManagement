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
		PURPOSE: 
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            select * 
            from Departments d
            where d.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {

                Department department = (await conn.QueryAsync<Department>(sql)).ToList().Single();

                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
        }
    }
}