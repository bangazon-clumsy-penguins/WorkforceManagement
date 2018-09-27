﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkforceManagement.Models
{

	public class Computer
	{
		[Key]
		public int Id { get; set; }

		public string Model { get; set; }

		public DateTime PurchaseDate { get; set; }

		public DateTime? DecommissionDate { get; set; }

	}
}