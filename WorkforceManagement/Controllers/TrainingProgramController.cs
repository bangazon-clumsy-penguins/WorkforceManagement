﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;
using System.Data.SqlClient;

namespace WorkforceManagement.Controllers
{
    /*
        AUTHORS: Phillip Patton, April Watson
        PURPOSE: To prescribe available actions to the user pertaining to viewing, editing and creating Training Programs.
    */
    public class TrainingProgramController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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

        // GET: TrainingProgram
        public async Task<IActionResult> Index()
        {
            string sql = @"
            select
                t.Id
                ,t.Name
            from Trainings t
            ";

            using (IDbConnection conn = Connection)
            {
                Dictionary<int, TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();

                var TrainingProgramsQuerySet = await conn.QueryAsync<TrainingProgram>(
                        sql
                    );
                return View(TrainingProgramsQuerySet);

            }
        }

        // GET: TrainingProgram/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
            select
                t.Id
                ,t.Name
                ,t.StartDate
                ,t.EndDate
                ,t.MaxOccupancy
            from Trainings t
            where t.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                TrainingProgram trainingProgram = await conn.QuerySingleAsync<TrainingProgram>(sql);

                if (trainingProgram == null)
                {
                    return NotFound();
                }

                return View(trainingProgram);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingProgram trainingProgram)
        {

            if (ModelState.IsValid)
            {
                string sql = $@"
                    INSERT INTO Trainings
                        ( Name, StartDate, EndDate, MaxOccupancy )
                        VALUES
                        (  '{trainingProgram.Name}'
                            , '{trainingProgram.StartDate}'
                            , '{trainingProgram.EndDate}'
                            , {trainingProgram.MaxOccupancy}
                        )
                    ";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(trainingProgram);
            
        }

        // GET: TrainingProgram/Edit/5
        // Retrieves information pertaining to the selected training program for the purpose of allowing the user to edit
        public async Task<IActionResult> Edit (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
                Select
                    Id,
                    Name,
                    Description,
                    StartDate,
                    EndDate,
                    MaxOccupancy
                From Trainings
                Where Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                TrainingProgram train = new TrainingProgram();

                var trainingQuery = (await conn.QueryAsync<TrainingProgram>(
                    sql)).Single();
                train = trainingQuery;

                return View(train);
            }
        }

        // POST: TrainingProgram/Edit/5
        // Posts user's changes to the database for the specified training program
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int id, TrainingProgram model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = $@"
                UPDATE Trainings
                SET Name = '{model.Name}',
                    StartDate = '{model.StartDate.Date}',
                    EndDate = '{model.EndDate.Date}',
                    MaxOccupancy = {model.MaxOccupancy}
                WHERE Id = {id}";

                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    throw new Exception("No rows affected");
                }
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            }
        }

        // GET: TrainingProgram/Delete/5
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string sql = $@"
                select
                    Id,
                    Name,
                    Description,
                    StartDate,
                    EndDate,
                    MaxOccupancy
                from Trainings
                WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {

                TrainingProgram trainingQuery = (await conn.QueryAsync<TrainingProgram>(sql)).ToList().Single();

                if (trainingQuery == null)
                {
                    return NotFound();
                }

                return View(trainingQuery);
            }
        }

        // POST: TrainingProgram/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            string sql = $@"DELETE FROM EmployeeTrainings WHERE TrainingId = {id};
                    Delete from Trainings where Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                throw new Exception("No rows affected");
            }
        }
    }
}