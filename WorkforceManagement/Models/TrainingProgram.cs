﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace WorkforceManagement.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string Name { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Occupancy must be greater than 0!")]
        [Display(Name = "Max Occupancy")]
        public int MaxOccupancy { get; set; }

        [Display(Name = "Assigned Employees")]
        public List<Employee> AssignedEmployees { get; set; } = new List<Employee>();

    }
}
