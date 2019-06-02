using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    public class Appointments
    {
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        //by using 'NotMapped' the AppointmentTime will not be added to database 
        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public bool isConfirmed { get; set; }



    }
}
