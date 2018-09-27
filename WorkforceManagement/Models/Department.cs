using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models
{
	/* 
		AUTHOR: Elliot Huck
		PURPOSE: To model a department of the company. The Name and Budget are required properties and the Budget must be a non-negative double.
	*/

	public class Department
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "Department")]
		public string Name { get; set; }

		[Required]
		[Range(0.0, Double.PositiveInfinity)]
		public double Budget { get; set; }

		public List<Employee> EmployeeList { get; set; }
	}
}
