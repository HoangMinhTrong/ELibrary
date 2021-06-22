﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ELibrary_Team_1.Models
{
    public class AppUser:IdentityUser
    {
        //

        public string FullName { get; set; }
        public ICollection<AccessRequest> AccessRequests { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<UpdateRequest> UpdateRequests { get; set; }

       


    }
    


}
