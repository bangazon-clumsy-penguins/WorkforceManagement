using System;
using System.Collections.Generic;
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

        public List<Training> AssignableTrainings { get; set; } = new List<Training>();

        public List<Training> AssignedTrainings { get; set; } = new List<Training>();

        public List<Computer> AvailableComputers { get; set; } = new List<Computer>();

        public Employee Employee { get; set; }

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
            LEFT OUTER JOIN (
                
                SELECT
                    t.Id
                from Trainings t
                LEFT OUTER JOIN EmployeeTrainings et on et.TrainingId = t.Id
                WHERE et.EmployeeId = {id}
                GROUP BY t.Id
            
            ) b on b.Id = t.Id
            WHERE b.Id is null
            and t.startdate >= '{DateTime.Now.ToString("yyyy-MM-dd")}';";

            string availableComputers = $@"Select c.Id,
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
                AssignedTrainings = (conn.Query<Training>(assignedTrainings)).ToList();
                AssignableTrainings = (conn.Query<Training>(assignableTrainings)).ToList();

                this.DepartmentList = department
                    .Select(li => new SelectListItem
                    {
                        Text = li.Name,
                        Value = li.Id.ToString()
                    }).ToList();
            }
        }
    }
}
