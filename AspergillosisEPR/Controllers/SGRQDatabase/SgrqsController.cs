using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Controllers.SGRQDatabase
{
    public class SgrqsController : Controller
    {
        private readonly AspergillosisContext _context;

        public SgrqsController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult Index(int startId)
        {
            return Ok();
        }

    }
}