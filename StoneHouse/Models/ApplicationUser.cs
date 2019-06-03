using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    public class ApplicationUser : IdentityUser
    {
        //this property will be added to the asp.net users table
        [Display(Name="Sales Person")]
        public string Name { get; set; }

        //the data anootation called 'Notmapped' will not add this property the the asp.net users table 
        [NotMapped]
        public bool isSuperAdmin { get; set; }
    }
}
