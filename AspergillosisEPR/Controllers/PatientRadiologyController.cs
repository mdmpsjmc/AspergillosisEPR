using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    public class PatientRadiologyController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientRadiologyController(AspergillosisContext context)
        {
            _context = context;
        }      
    }
}