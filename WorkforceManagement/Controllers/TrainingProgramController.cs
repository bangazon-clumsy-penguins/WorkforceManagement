using System;
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    StartDate,
                    EndDate,
                    MaxOccupancy
                From Trainings
                Where Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                TrainingProgram model = new TrainingProgram();

                var training = (await conn.QueryAsync<TrainingProgram>(
                    sql)).Single();
                model = training;
                return View(model);
            }
        }

        // POST: TrainingProgram/Edit/5
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

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
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