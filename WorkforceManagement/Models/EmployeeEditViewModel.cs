using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace WorkforceManagement.Models
{
    public class EmployeeEditViewModel
    {
        private readonly IConfiguration _config;

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<SelectListItem> AssignableTrainings { get; set; } = new List<SelectListItem>();

        public List<string> AssignedTrainings { get; set; } = new List<string>();

        [Display(Name="Register Trainings (Ctrl + click to select multiple)")]
        public MultiSelectList TrainingMulti { get; set; }

        [Display(Name="Current Computer")]
        public List<SelectListItem> AvailableComputers { get; set; } = new List<SelectListItem>();

        public Employee Employee { get; set; }

        [Display(Name="Department")]
        public List<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();

        public EmployeeEditViewModel() { }

        public EmployeeEditViewModel(IConfiguration config, int id)
        {
            _config = config;

            string sql = $@"SELECT Id, Name FROM Departments";

            string assignedTrainings = $@"
            SELECT 
                t.Id
                ,t.Name
                ,t.Description
                ,t.StartDate
            from Trainings t
            LEFT OUTER JOIN EmployeeTrainings et on et.TrainingId = t.Id
            WHERE et.EmployeeId = {id}
            and t.startdate >= '{DateTime.Now.ToString("yyyy-MM-dd")}'
            GROUP BY t.Id
                ,t.Name
                ,t.Description
                ,t.StartDate;";

            string assignableTrainings = $@"
            SELECT t.Id
                ,t.Name
                ,t.Description
                ,t.StartDate
            from Trainings t
            where t.startdate >= '{DateTime.Now.ToString("yyyy-MM-dd")}';";

            string availableComputers = $@"
            Select c.Id,
                c.Manufacturer,
                c.Model,
                c.PurchaseDate,
                c.DecommissionDate
            From Computers c
                Left Join (
	                Select *
	                From EmployeeComputers ec
	                Where ec.ReturnDate is null
                ) r on c.Id = r.ComputerId
            Where r.ComputerId is null or r.EmployeeId = {id};";

            using (IDbConnection conn = Connection)
            {
                List<Department> department = (conn.Query<Department>(sql)).ToList();
                List<Computer> computers = (conn.Query<Computer>(availableComputers)).ToList();
                List<Training> currentTrainings = (conn.Query<Training>(assignedTrainings)).ToList();
                List<Training> usableTraining = (conn.Query<Training>(assignableTrainings)).ToList();

                this.DepartmentList = department
                    .Select(li => new SelectListItem
                    {
                        Text = li.Name,
                        Value = li.Id.ToString()
                    }).ToList();

                this.AssignedTrainings = currentTrainings
                    .Select(li => li.Id.ToString()).ToList();

                this.AssignableTrainings = usableTraining
                    .Select(li => new SelectListItem
                    {
                        Text = li.Name,
                        Value = li.Id.ToString()
                    }).ToList();

                this.AvailableComputers = computers
                .Select(li => new SelectListItem
                {
                    Text = $"{li.Manufacturer} {li.Model}",
                    Value = li.Id.ToString()
                }).ToList();

                AvailableComputers.Insert(0, new SelectListItem
                {
                    Text = "None",
                    Value = "0"
                });


                TrainingMulti = new MultiSelectList(AssignableTrainings.OrderBy(x => x.Text), "Value", "Text");


            }
        }
    }
}
