using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace WorkforceManagement.Models
{
    /*
        AUTHORS: Phillip Patton
        PURPOSE: To model a training program for the company. Each program has a name, description, start date, end date and max occupancy.
    */

    public class TrainingProgram : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (StartDate < DateTime.Today)
            {
                results.Add(new ValidationResult("Start date and time must be greater than current time", new[] { "StartDate" }));
            }

            if (EndDate < StartDate)
            {
                results.Add(new ValidationResult("End Date must be greater that Start Date", new[] { "EndDate" }));
            }

            return results;
        }
    }
}
