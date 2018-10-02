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

		public IDbConnection Connection
		{
			get
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
				}
				else
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
			}
			else
			{
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
					}
					else
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

		/* GET: Computer/Delete/5
		This GET method is launched by clicking the "Delete" link in the Details view of an individual computer.
		The method uses the ComputerHasBeenAssigned() method below to check if the computer has been assigned to any employees
		If it has, the user is redirected to the DenyDelete view informing them they cannot delete the computer
		If it has not been assigned, the user is redirected to the ConfirmDelete view prompting them to confirm that they want to delete the item.
			If the Delete button is clicked, the computer is deleted and the user is redirected to the Index
			If the Cancel button is clicked, the computer is NOT deleted and the user redirected to the Index
		 */
		[HttpGet]
		public async Task<IActionResult> ConfirmDelete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			string sql = $@"
			SELECT
				c.Id,
				c.Manufacturer,
				c.Model,
				c.PurchaseDate,
				c.DecommissionDate
			FROM Computers c
			WHERE c.Id = {id}
			";

			using (IDbConnection conn = Connection)
			{
				if (ComputerHasBeenAssigned(id))
				{
					return View("DenyDelete");
				}
				else
				{
					Computer computer = (await conn.QueryAsync<Computer>(sql)).Single();
					return View("ConfirmDelete", computer);
				}
			}
		}

		/*
		 This method checks if a computer has already been assigned to an employee. It is used by the ConfirmDelete and Delete methods.
		 */
		private bool ComputerHasBeenAssigned(int? id)
		{
			string sql = $@"
			SELECT 
				e.Id,
				e.FirstName,
				e.LastName,
				e.HireDate,
				e.DepartmentId
			FROM Employees e
			JOIN EmployeeComputers ec ON e.Id = ec.EmployeeId
			JOIN Computers c ON ec.ComputerId = c.Id
			WHERE c.Id = {id}
			";

			using (IDbConnection conn = Connection)
			{
				int numAssignments = (conn.Query<Employee>(sql)).Count();
				return numAssignments > 0;
			}
		}

		/* 
		 POST: Computer/Delete/5
		 This POST method removes the computer from the database. It is accessed by clicking the Delete button on the ConfirmDelete view
		 It double checks to make sure the computer has not been assigned to an employee, throwing an exception if it has been assigned.
		*/
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (ComputerHasBeenAssigned(id))
			{
				throw new Exception("You cannot delete a computer that has been assigned to an employee");
			}

			string sql = $@"
				DELETE FROM Computers WHERE Id = {id};
				";

			using (IDbConnection conn = Connection)
			{
				int rowsAffected = await conn.ExecuteAsync(sql);
				bool deleteSuccessful = rowsAffected > 0;

				if (deleteSuccessful)
				{
					return RedirectToAction(nameof(Index));
				} else {
					throw new Exception("No rows affected");
				}
			}
		}


	}
}