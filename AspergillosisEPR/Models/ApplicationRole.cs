using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IPAddress { get; set; }
    }
}