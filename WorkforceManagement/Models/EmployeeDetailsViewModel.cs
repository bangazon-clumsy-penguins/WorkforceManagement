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
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; } = new Employee();

        [Display(Name = "Training Programs")]
        public List<Training> Trainings { get; set; } = new List<Training>();

        private readonly IConfiguration _config;

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeDetailsViewModel() { }

        public EmployeeDetailsViewModel(IConfiguration config)
        {
            _config = config;

        }


            // Add a prompt so that the <select> element isn't blank
            //this.Cohorts.Insert(0, new SelectListItem
            //{
            //    Text = "Choose cohort...",
            //    Value = "0"
            //});
        }




}
