﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models.ViewModel
{
    public class AppointmentViewModel
    {
        public List<Appointments> Appointments { get; set; }

        //need the 'PagingInfo' for pagination
        public PagingInfo PagingInfo { get; set; }


    }
}
