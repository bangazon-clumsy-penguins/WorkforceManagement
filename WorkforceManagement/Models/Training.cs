﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkforceManagement.Models
{
    /* 
     AUTHORED: Adam Wieckert, Seth Dana, Elliot Huck, Evan Lusky, Phil Patton

     PURPOSE: Model to reflect the items on the Trainings Table in the BangazonAPI DB
    */
    public class Training
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Training")]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MaxOccupancy { get; set; }

        public List<Employee> RegisteredEmployees = new List<Employee>();
    }
}