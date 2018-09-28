using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkforceManagement.Models
{
    /* 
     AUTHORED: Adam Wieckert, Seth Dana, Elliot Huck, Evan Lusky, Phil Patton

     PURPOSE: Model to reflect the items on the Employees Table in the BangazonAPI DB
    */
    public class Employee
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Hire Date")]
        [Required]
        [HireDateValidation]
        public DateTime HireDate { get; set; }

        public bool IsSupervisor { get; set; }

        [Required]
        [Range(1,100000, ErrorMessage = "Please choose a department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public Computer Computer { get; set; }
    }

    public class HireDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Employee employee = (Employee)validationContext.ObjectInstance;

            if (employee.HireDate > DateTime.Now)
            {
                return new ValidationResult("Date of hire cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}