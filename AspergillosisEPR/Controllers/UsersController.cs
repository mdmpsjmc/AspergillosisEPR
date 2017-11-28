using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Controllers
{

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return View();
        }
    }
}