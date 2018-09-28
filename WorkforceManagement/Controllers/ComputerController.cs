using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;
using Dapper;

namespace WorkforceManagement.Controllers
{
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection { get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Computer
        // This GET method returns all the computers and provides them to View/Computer/Index.cshtml as a List<Computer>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string sql = $@"SELECT
                            c.Id,
                            c.Manufacturer,
                            c.Model,
                            c.PurchaseDate,
                            c.DecommissionDate
                            FROM Computers c;";

            using (IDbConnection conn = Connection)
            {
                List<Computer> computers = (await conn.QueryAsync<Computer>(sql)).ToList();
                return View(computers);
            }
        }

        // GET: Computer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

		// GET: Computer/Create
		// This GET method displays the form used to create a new computer
		public ActionResult Create()
        {
            return View();
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Computer newComputer)
        {

			if (!(ModelState.IsValid))
			{
				return View(newComputer);
			} else {
				string sql = $@"
				INSERT INTO Computers
					(Manufacturer, Model, PurchaseDate, DecommissionDate)
				VALUES
					('{newComputer.Manufacturer}', '{newComputer.Model}', '{newComputer.PurchaseDate}', null)
				";

				using (IDbConnection conn = Connection)
				{
					int rowsAffected = await conn.ExecuteAsync(sql);
					bool createdSuccessfully = rowsAffected > 0;
					
					if (createdSuccessfully)
					{
						return RedirectToAction(nameof(Index));
					} else
					{
						throw new Exception("No rows affected; record was not added to database");
					}
				}
			}


        }

        // GET: Computer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Computer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}