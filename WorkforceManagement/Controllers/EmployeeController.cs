using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;

namespace WorkforceManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            string sql = @"
            SELECT
                e.Id,
                e.FirstName,
                e.LastName,
                d.Id,
                d.Name
            FROM Employees e
            JOIN Departments d ON e.DepartmentId = d.Id;
        ";
            using (IDbConnection conn = Connection)
            {
                Dictionary<int, Employee> Employees = new Dictionary<int, Employee>();

                var EmployeeQuerySet = await conn.QueryAsync<Employee, Department, Employee>(
                        sql,
                        (employee, department) =>
                        {
                            if (!Employees.ContainsKey(employee.Id))
                            {
                                Employees[employee.Id] = employee;
                            }
                            Employees[employee.Id].Department = department;
                            return employee;
                        }
                    );
                return View(Employees.Values);

            }

            //return View();
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"SELECT e.Id, e.FirstName, e.LastName, d.Id, d.Name, c.Id, c.Model, t.Id, t.Name FROM Employees e
                            JOIN Departments d ON e.DepartmentId = d.Id
                            LEFT JOIN EmployeeComputers ec ON ec.EmployeeId = e.Id
                            LEFT JOIN Computers c ON ec.ComputerId = c.Id
                            LEFT JOIN EmployeeTrainings et ON et.EmployeeId = e.Id
                            LEFT JOIN Trainings t ON et.TrainingId = t.Id
                            WHERE e.Id = {id};";

            using (IDbConnection conn = Connection)
            {
                EmployeeDetailsViewModel model = new EmployeeDetailsViewModel(_config);


                var EmployeeQuery = await conn.QueryAsync<Employee, Department, Computer, Training, Employee>(
                    sql, (employee, department, computer, training) =>
                {
                    model.Employee = employee;
                    model.Employee.Computer = computer;
                    model.Employee.Department = department;
                    model.Trainings.Add(training);
                    return employee;
                });

                return View(model);
            }
        }
        //return View();

        //GET: Employee/Create
        public ActionResult Create()
        {
            EmployeeCreateViewModel createModel = new EmployeeCreateViewModel(_config);
            return View(createModel);
        }

        //POST: Employee/Create
       //Should Include FirstName, LastName, StartDate, and Dropdown with Departments
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                    // TODO: Add insert logic here
                    string sql = $@"INSERT INTO Employees
                                (FirstName, LastName, HireDate, DepartmentId, IsSupervisor)
                                 VALUES ('{employee.FirstName}', '{employee.LastName}', '{employee.HireDate}', '{employee.DepartmentId}', 0);";
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                EmployeeCreateViewModel createModel = new EmployeeCreateViewModel(_config);
                return View(createModel);
            }
            
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            return View((await getEmployeeEditViewModel(id)));
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel editedEmployee)
        {
            EmployeeEditViewModel currentEmployee = await getEmployeeEditViewModel(id);

			using (IDbConnection conn = Connection)
			{ 

		// This section handles changes to the last name
				if (editedEmployee.Employee.LastName != currentEmployee.Employee.LastName)
				{
					string updateLastName = $@"UPDATE Employees SET LastName = '{editedEmployee.Employee.LastName}'
											WHERE Id = {id}";
				}


		// This section handles changes to the department
				if (editedEmployee.Employee.DepartmentId != currentEmployee.Employee.DepartmentId)
				{
					string updateDepartment = $@"UPDATE Employees SET DepartmentId = {editedEmployee.Employee.DepartmentId}
												WHERE Id = {id}";
				}

		// This section handles changes to the computer
	
				if (currentEmployee.Employee.Computer != null)
				{ // Runs if the employee has a computer already

					if (editedEmployee.Employee.Computer.Id != currentEmployee.Employee.Computer.Id)
					{ // Runs if the employee has changed to a new computer or changed to no computer
						// Sets the ReturnDate of their old computer to today
						string returnComputer = $@"
						UPDATE EmployeeComputers SET ReturnDate = '{DateTime.Today}'
						WHERE EmployeeId = {id} AND ComputerId = {currentEmployee.Employee.Computer.Id}
						AND ReturnDate IS NULL;
						";
						bool computerReturnSuccess = (await conn.ExecuteAsync(returnComputer)) > 0;

						if (editedEmployee.Employee.Computer.Id != 0)
						{ // Runs if the employee has changed to a new computer
							// Adds the new computer into the EmployeeComputer intersection table
							string assignEmployeeComputer = $@"
							INSERT INTO EmployeeComputers 
							VALUES 
								('{DateTime.Today}', null, {id}, {editedEmployee.Employee.Computer.Id});
							";
							bool computerChangeSuccess = (await conn.ExecuteAsync(assignEmployeeComputer)) > 0;
						}
					}
				}
				else
				{ // Runs if the employee did not already have a computer
					if (editedEmployee.Employee.Computer.Id != 0)
					{ // Runs if the employee is assigned a new computer
						string assignEmployeeComputer = $@"
						INSERT INTO EmployeeComputers
						VALUES 
							('{DateTime.Today}', null, {id}, {editedEmployee.Employee.Computer.Id});
						";
						bool computerAddSuccess = (await conn.ExecuteAsync(assignEmployeeComputer)) > 0;
					}
					// If the employee had no computer and was not assigned a computer, nothing happens
				}


		// This section handles changes to the trainings
				IEnumerable<string> removedTrainings = currentEmployee.AssignedTrainings.Except(editedEmployee.AssignedTrainings);
				IEnumerable<string> addedTrainings = editedEmployee.AssignedTrainings.Except(currentEmployee.AssignedTrainings);

				StringBuilder allSql = new StringBuilder();
				foreach (string trainingId in removedTrainings)
				{
					string sql = $@"
					DELETE FROM EmployeeTrainings 
					WHERE EmployeeId = {id}
					AND TrainingId = {Int32.Parse(trainingId)}; 
					";

					allSql.Append(sql);
				}

				foreach (string trainingId in addedTrainings)
				{
					string sql = $@"
					INSERT INTO EmployeeTrainings
						(EmployeeId, TrainingId)
					VALUES
						('{id}', '{trainingId}');  
					";

					allSql.Append(sql);
				}

				if (allSql.Length > 0)
				{
					int rowsAffected = await conn.ExecuteAsync(allSql.ToString());
				}
			}

			return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
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

        public async Task<EmployeeEditViewModel> getEmployeeEditViewModel (int id)
        {
            string sql = $@"SELECT 
                e.Id,
                e.FirstName,
                e.LastName,
                e.HireDate,
                e.DepartmentId
            FROM Employees e
            WHERE e.Id = {id};";

            string currentComputer = $@"SELECT
                    c.Id,
                    c.Manufacturer,
                    c.Model,
                    c.PurchaseDate
                    FROM Computers c
                    Join EmployeeComputers ec on c.Id = ec.ComputerId
                    Where ec.EmployeeId = {id} AND ec.ReturnDate is null;";

            using (IDbConnection conn = Connection)
            {

                Employee employeeToAdd = (await conn.QueryAsync<Employee>(sql)).Single();

                var employeeComputer = (await conn.QueryAsync<Computer>(currentComputer));

                if (employeeComputer.Count() == 1)
                {
                    employeeToAdd.Computer = employeeComputer.Single();
                }
                EmployeeEditViewModel employeeEditModel = new EmployeeEditViewModel(_config, id);
                employeeEditModel.Employee = employeeToAdd;
                return employeeEditModel;
            }
        }
    }
}