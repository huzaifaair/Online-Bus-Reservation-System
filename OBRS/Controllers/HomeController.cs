using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBRS.Models;

namespace OBRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly obrsContext _obrsContext;

        public HomeController(ILogger<HomeController> logger, obrsContext newObrsContext)
        {
            _logger = logger;
            _obrsContext = newObrsContext;
        }

        public IActionResult Index()
        {
            var buses = _obrsContext.tbl_bus
                .Include(b => b.routes)
                .Include(b => b.price)
                .ToList();

            return View(buses);
        }

        public IActionResult About()
        {
            return View();
        }

        // 🔹 Search Buses Page (GET)
        [HttpGet]
        public IActionResult SearchBuses(string fromCity, string toCity)
        {
            // Fetch unique cities from routes (StartLocation + Destination)
            ViewBag.Cities = _obrsContext.tbl_route
                .Select(r => r.StartLocation)
                .Union(_obrsContext.tbl_route.Select(r => r.Destination))
                .Distinct()
                .ToList();

            var buses = _obrsContext.tbl_bus
                .Include(b => b.routes)
                .Include(b => b.price)
                .AsQueryable();

            if (!string.IsNullOrEmpty(fromCity))
            {
                buses = buses.Where(b => b.routes.StartLocation == fromCity);
            }
            if (!string.IsNullOrEmpty(toCity))
            {
                buses = buses.Where(b => b.routes.Destination == toCity);
            }

            return View(buses.ToList());
        }

        [Authorize(Roles = "user")]
        public IActionResult BookTicket(Guid busId)
        {
            return RedirectToAction("Create", "Booking", new { busId = busId });
        }

        [Authorize(Roles = "user")]
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(Contact contact, [FromServices] obrsContext _context)
        {
            if (ModelState.IsValid)
            {
                _context.tbl_contact.Add(contact);
                _context.SaveChanges();

                TempData["Success"] = "✅ Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }

            return View(contact);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
