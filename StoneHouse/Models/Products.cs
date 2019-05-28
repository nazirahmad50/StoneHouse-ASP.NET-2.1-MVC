using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    public class Products
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public string Image { get; set; }
        public string ShadeColor { get; set; }


        //ProductTypes foreign key
        [Display(Name = "Produc Type")]
        public int ProductTypeId { get; set; }

        //the table the foreign key should reference to
        [ForeignKey("ProductTypeId")]
        //create virtual property for the ProductTypes
        //if the virtual keyword is used that means that ProductTypes wont be added to the database
        public virtual ProductTypes ProductTypes { get; set; }


        [Display(Name = "Special Tag")]
        public int SpecialTagId { get; set; }

        [ForeignKey("SpecialTagId")]
        public virtual SpecialTags SpecialTags { get; set; }


    }
}
