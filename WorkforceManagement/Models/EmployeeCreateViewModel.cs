using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using WorkforceManagement.Models;

namespace WorkforceManagement.Models
{
    public class EmployeeCreateViewModel
    {
        [Required]
        public Employee Employee { get; set; } = new Employee();

        [Display(Name = "Departments")]
        public List<SelectListItem> DepartmentsList { get; } = new List<SelectListItem>();

        private readonly IConfiguration _config;

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeCreateViewModel() { }

        public EmployeeCreateViewModel(IConfiguration config)
        {
            _config = config;

            string sql = $@"SELECT Id, Name FROM Departments";

            using (IDbConnection conn = Connection)
            {
                List<Department> department = (conn.Query<Department>(sql)).ToList();

                this.DepartmentsList = department
                    .Select(li => new SelectListItem
                    {
                        Text = li.Name,
                        Value = li.Id.ToString()
                    }).ToList();
            }


            // Add a prompt so that the <select> element isn't blank
            this.DepartmentsList.Insert(0, new SelectListItem
            {
                Text = "Choose department...",
                Value = "0"
            });
        }

    }
}
