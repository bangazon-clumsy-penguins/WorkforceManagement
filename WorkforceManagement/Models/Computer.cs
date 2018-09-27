using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public string Model { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DecommissionDate { get; set; }
    }
}
