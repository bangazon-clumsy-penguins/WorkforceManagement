﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkforceManagement.Models
{
	public class Employee
	{

		[Key]
		public int Id { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public DateTime HireDate { get; set; }

		public bool IsSupervisor { get; set; }

		public int DepartmentId { get; set; }

		public Department Department { get; set; }

		public Computer Computer { get; set; }

	}
}
