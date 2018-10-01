using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/* 
AUTHORED BY: Adam Wieckert

The Computer model has proporties associated with the Computers table within the BangazonAPI DB. 
*/

namespace WorkforceManagement.Models
{
    public class Computer
    {
        [Key]
        public int Id { get; set; }

		[Required]
        public string Manufacturer { get; set; }

		[Required]
        public string Model { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Decommission Date")]
        public DateTime? DecommissionDate { get; set; }
    }
}
