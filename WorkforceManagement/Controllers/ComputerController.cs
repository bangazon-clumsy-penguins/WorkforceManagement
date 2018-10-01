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
        // Query the database for a specific computer based on the id provided at the end of the url
        public async Task<IActionResult> Details(int id)
        {
            string sql = $@"SELECT
                            c.Id,
                            c.Manufacturer,
                            c.Model,
                            c.PurchaseDate,
                            c.DecommissionDate
                            FROM Computers c
                            WHERE c.Id = {id};";

            using (IDbConnection conn = Connection)
            {
                if (CheckComputerDoesNotExist(id))
                {
                    return RedirectToAction(nameof(Index));
                } else
                {
                    var computer = (await conn.QueryAsync<Computer>(sql)).Single();
                    return View(computer);
                }
            }
        }

        // Checks to see if the computer Id in question does not exist in the data base. 
        // If the computer does not exist the function will return True
        private bool CheckComputerDoesNotExist(int id)
        {
            string sql = $"SELECT * FROM Computers c WHERE c.Id = {id};";

            using (IDbConnection conn = Connection)
            {
                var theCount = conn.Query<Computer>(sql).Count();
                return theCount == 0;
            }
        }

        // GET: Computer/Create
        // This GET method displays the form used to create a new computer
        public ActionResult Create()
        {
            return View();
        }

        // POST: Computer/Create
		// This POST method attempts to post a new computer to the database using the information entered in the form
		// If the information entered is not valid, it will show the GET view with the currently entered information, along with any applicable error messages
		// If the data is successfully posted, it will redirect to the main Index view
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