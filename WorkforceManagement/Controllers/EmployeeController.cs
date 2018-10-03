﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                Computer employeeComputer = (await conn.QueryAsync<Computer>(currentComputer)).Single();
                Employee employeeToAdd = (await conn.QueryAsync<Employee>(sql)).Single();
                EmployeeEditViewModel employeeEditModel = new EmployeeEditViewModel(_config, id);
                employeeToAdd.Computer = employeeComputer;
                employeeEditModel.Employee = employeeToAdd;
                return View(employeeEditModel);
            }
        }

        // POST: Employee/Edit/5
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
    }
}