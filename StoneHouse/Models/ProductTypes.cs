using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    public class ProductTypes
    {

        //The id property doesnt need required attribute as it will be generated automatically by the database
        //When data is migrated ASP.Net will be smart enough to put the Id as the primary key in database
        //So it will be automatically incremented 
        public int Id { get; set; }
        //The required attribute means that a new product must have a name
        [Required]
        public string Name { get; set; }
    }
}
