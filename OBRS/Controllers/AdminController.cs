using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBRS.Areas.Identity.Data;
using OBRS.Data;
using OBRS.Models;


namespace OBRS.Controllers
{
    public class AdminController : Controller
    {
        private readonly obrsContext _obrsContext;
        private readonly UserManager<OBRSUser> _userManager;
        private readonly OBRSContext oBRSContext;
        private readonly IWebHostEnvironment env;

        public AdminController(obrsContext NewContext, IWebHostEnvironment newenv, OBRSContext newContext, UserManager<OBRSUser> userManager)
        {
            this._userManager = userManager;
            this._obrsContext = NewContext;
            this.oBRSContext = newContext;
            this.env = newenv;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            // 🔹 Dashboard Widgets (Simple Counts)
            ViewBag.BusesCount = _obrsContext.tbl_bus.Count();
            ViewBag.RoutesCount = _obrsContext.tbl_route.Count();
            ViewBag.EmployeesCount = _obrsContext.tbl_employees.Count();
            ViewBag.BookingsCount = _obrsContext.tbl_bookings.Count();

            // 🔹 Revenue = Sum of FinalFare from confirmed bookings
            var revenue = _obrsContext.tbl_bookings
                            .Where(b => b.Status == "Confirmed")
                            .Sum(b => (decimal?)b.bus.price.FinalFare) ?? 0;

            ViewBag.Revenue = revenue;
            ;

            // 🔹 Upcoming Departures (Next 5 Trips)
            var now = DateTime.Now;
            var upcomingTrips = _obrsContext.tbl_bus
                                .Include(b => b.routes)
                                .Where(b => b.DepartureTime > now)
                                .OrderBy(b => b.DepartureTime)
                                .Take(5)
                                .Select(b => new
                                {
                                    b.BusNumber,
                                    RouteName = b.routes.RouteName,
                                    b.DepartureTime,
                                    b.ArrivalTime,
                                    b.AvailableSeats
                                })
                                .ToList();

            ViewBag.UpcomingTrips = upcomingTrips;

            return View();
        }



        //Admin Dashboard End

        [Authorize(Roles = "admin")]
        //Start Add Buses CRUD
        public IActionResult AddBuses()
        {
            var routebuses = _obrsContext.tbl_route.ToList();
            ViewBag.Routebuses = new SelectList(routebuses, "RouteId", "RouteName");
            return View();
           
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddBuses(Buses b, IFormFile BusImage)
        {
            if (BusImage != null && BusImage.Length > 0)
            {
                string filename = Path.GetFileName(BusImage.FileName);
                string filepath = Path.Combine(env.WebRootPath, "images", filename);
                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    BusImage.CopyTo(fs);
                }
                b.BusImage = filename;
            }


            _obrsContext.tbl_bus.Add(b);
            _obrsContext.SaveChanges();
            TempData["success"] = "Bus has been added successfully";
            return RedirectToAction("ManageBuses", "Admin");
        }

        [Authorize(Roles = "admin")]
        public IActionResult ManageBuses()
        {
            
            var buses = _obrsContext.tbl_bus.Include(b => b.routes).ToList();
            return View(buses);
        }
        [Authorize(Roles = "admin")]
        public IActionResult UpdateBuses(Guid id)
            {
                var buses = _obrsContext.tbl_bus.Find(id);
                var routebuses = _obrsContext.tbl_route.ToList();
                ViewBag.Routebuses = new SelectList(routebuses, "RouteId", "RouteName");
                return View(buses);
            }
        [Authorize(Roles = "admin")]
        [HttpPost]
            public IActionResult UpdateBuses(Buses b, IFormFile BusImage)
            {
                var existingBus = _obrsContext.tbl_bus.FirstOrDefault(x => x.BusId == b.BusId);
                if (existingBus == null) return NotFound();

                if (BusImage != null && BusImage.Length > 0)
                {
                    // ✅ Delete old image if it exists
                    if (!string.IsNullOrEmpty(existingBus.BusImage))
                    {
                        string oldFilePath = Path.Combine(env.WebRootPath, "images", existingBus.BusImage);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // ✅ Save new image
                    string filename = Path.GetFileName(BusImage.FileName);
                    string filepath = Path.Combine(env.WebRootPath, "images", filename);

                    using (var fs = new FileStream(filepath, FileMode.Create))
                    {
                        BusImage.CopyTo(fs);
                    }

                    existingBus.BusImage = filename;
                }

                // ✅ Update other properties manually (safe way)
                existingBus.BusNumber = b.BusNumber;
                existingBus.BusName = b.BusName;
                existingBus.BusType = b.BusType;
                existingBus.TotalSeats = b.TotalSeats;
                existingBus.AvailableSeats = b.AvailableSeats;
                existingBus.DepartureTime = b.DepartureTime;
                existingBus.ArrivalTime = b.ArrivalTime;
                existingBus.PricePerSeat = b.PricePerSeat;
                existingBus.IsActive = b.IsActive;
                existingBus.Route_id = b.Route_id;

                _obrsContext.SaveChanges();
                TempData["update"] = "Bus details have been updated successfully";
                return RedirectToAction("ManageBuses", "Admin");
            }
        [Authorize(Roles = "admin")]
        public IActionResult DeleteBuses(Guid id)
        {
            var buses = _obrsContext.tbl_bus.Find(id);
            _obrsContext.tbl_bus.Remove(buses);
            _obrsContext.SaveChanges();
            TempData["danger"] = "Bus has been deleted successfully";
            return RedirectToAction("ManageBuses", "Admin");
        }
        // End Add Buses CRUD
        [Authorize(Roles = "admin")]
        // Start Add Routes CRUD
        public IActionResult AddRoutes() 
        { 
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddRoutes(Routes r)
        {
            _obrsContext.tbl_route.Add(r);
            _obrsContext.SaveChanges();
            TempData["success"] = "Route has been added successfully";
            return RedirectToAction("ManageRoutes", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult ManageRoutes()
        {
            var routes = _obrsContext.tbl_route.ToList();
            return View(routes);
        }
        [Authorize(Roles = "admin")]
        public IActionResult UpdateRoutes(Guid id)
        {
            var routes = _obrsContext.tbl_route.Find(id);
            return View(routes);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult UpdateRoutes(Routes r)
        {
            _obrsContext.tbl_route.Update(r);
            _obrsContext.SaveChanges();
            TempData["update"] = "Route details has been updated successfully";
            return RedirectToAction("ManageRoutes", "Admin");

        }
        [Authorize(Roles = "admin")]
        public IActionResult DeleteRoutes(Guid id)
        {
            var routes = _obrsContext.tbl_route.Find(id);
            _obrsContext.tbl_route.Remove(routes);
            _obrsContext.SaveChanges();
            TempData["danger"] = "Route has been deleted successfully";
            return RedirectToAction("ManageRoutes", "Admin");
        }

        // End Routes CRUD
        [Authorize(Roles = "admin")]
        // Start Add price CRUD
        public IActionResult AddPrices()
        {
            var pricebuses = _obrsContext.tbl_bus
                .Include(b => b.routes)  // 👈 make sure you included route in Bus model
                .Select(b => new
                {
                    BusId = b.BusId,
                    DisplayName = b.BusNumber + " - " + b.routes.RouteName
                })
                .ToList();

            ViewBag.Pricebuses = new SelectList(pricebuses, "BusId", "DisplayName");
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddPrices(Prices p)
        {
            _obrsContext.tbl_price.Add(p);
            _obrsContext.SaveChanges();
            TempData["success"] = "Price has been added successfully";
            return RedirectToAction("ManagePrices", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult ManagePrices()
        {
            var prices = _obrsContext.tbl_price.Include(e => e.bus).ThenInclude(b => b.routes).ToList();
            return View(prices);
        }
        [Authorize(Roles = "admin")]
        public IActionResult UpdatePrices(Guid id)
        {
            var pricebuses = _obrsContext.tbl_bus
                .Include(b => b.routes)  // 👈 make sure you included route in Bus model
                .Select(b => new
                {
                    BusId = b.BusId,
                    DisplayName = b.BusNumber + " - " + b.routes.RouteName
                })
                .ToList();

            ViewBag.Pricebuses = new SelectList(pricebuses, "BusId", "DisplayName");
            var prices = _obrsContext.tbl_price.Find(id);
            return View(prices);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult UpdatePrices(Prices p)
        {
            _obrsContext.tbl_price.Update(p);
            _obrsContext.SaveChanges();
            TempData["update"] = "Price details has been updated successfully";
            return RedirectToAction("ManagePrices", "Admin");

        }
        [Authorize(Roles = "admin")]
        public IActionResult DeletePrices(Guid id)
        {
            var prices = _obrsContext.tbl_price.Find(id);
            _obrsContext.tbl_price.Remove(prices);
            _obrsContext.SaveChanges();
            TempData["danger"] = "Price has been deleted successfully";
            return RedirectToAction("ManagePrices", "Admin");
        }

        // End price CRUD

        //Start Add Employee CRUD
        [Authorize(Roles = "admin")]
        public IActionResult AddEmployees()
        {
            var empbuses = _obrsContext.tbl_bus
                .Include(b => b.routes)  // 👈 make sure you included route in Bus model
                .Select(b => new
                {
                    BusId = b.BusId,
                    DisplayName = b.BusNumber + " - " + b.routes.RouteName
                })
                .ToList();

            ViewBag.Empbuses = new SelectList(empbuses, "BusId", "DisplayName");

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddEmployees(Employee e, IFormFile EmployeeImage)
        {
            if (EmployeeImage != null && EmployeeImage.Length > 0)
            {
                string filename = Path.GetFileName(EmployeeImage.FileName);
                string filepath = Path.Combine(env.WebRootPath, "images", filename);
                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    EmployeeImage.CopyTo(fs);
                }
                e.EmployeeImage = filename;
            }


            _obrsContext.tbl_employees.Add(e);
            _obrsContext.SaveChanges();
            TempData["success"] = "Employee has been added successfully";
            return RedirectToAction("ManageEmployees", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult ManageEmployees()
        {
            var Employee = _obrsContext.tbl_employees.Include(e => e.bus).ThenInclude(b => b.routes).ToList();
            return View(Employee);
        }
        [Authorize(Roles = "admin")]
        public IActionResult UpdateEmployees(Guid id)
        {
            var empbuses = _obrsContext.tbl_bus
                .Include(b => b.routes)  // 👈 make sure you included route in Bus model
                .Select(b => new
                {
                    BusId = b.BusId,
                    DisplayName = b.BusNumber + " - " + b.routes.RouteName
                })
                .ToList();

            ViewBag.Empbuses = new SelectList(empbuses, "BusId", "DisplayName");
            var Employee = _obrsContext.tbl_employees.Find(id);
            return View(Employee);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult UpdateEmployees(Employee e, IFormFile EmployeeImage)
        {
            var existingEmp = _obrsContext.tbl_employees.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
            if (existingEmp == null) return NotFound();

            if (EmployeeImage != null && EmployeeImage.Length > 0)
            {
                // ✅ Delete old image if it exists
                if (!string.IsNullOrEmpty(existingEmp.EmployeeImage))
                {
                    string oldFilePath = Path.Combine(env.WebRootPath, "images", existingEmp.EmployeeImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // ✅ Save new image
                string filename = Path.GetFileName(EmployeeImage.FileName);
                string filepath = Path.Combine(env.WebRootPath, "images", filename);

                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    EmployeeImage.CopyTo(fs);
                }

                existingEmp.EmployeeImage = filename;
            }

            // ✅ Update other properties manually (safe way)

            existingEmp.FullName = e.FullName;
            existingEmp.CNIC = e.CNIC;
            existingEmp.PhoneNumber = e.PhoneNumber;
            existingEmp.Address = e.Address;
            existingEmp.Role = e.Role;
            existingEmp.LicenseNumber = e.LicenseNumber;
            existingEmp.HireDate = e.HireDate;
            existingEmp.IsActive = e.IsActive;
            existingEmp.Bus_id = e.Bus_id;

            _obrsContext.SaveChanges();
            TempData["update"] = "Employee details have been updated successfully";
            return RedirectToAction("ManageEmployees", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult DeleteEmployees(Guid id)
        {
            var Employee = _obrsContext.tbl_employees.Find(id);
            _obrsContext.tbl_employees.Remove(Employee);
            _obrsContext.SaveChanges();
            TempData["danger"] = "Employee has been deleted successfully";
            return RedirectToAction("ManageEmployees", "Admin");
        }
        // End Add Employee CRUD

        // Start Add booking CRUD
        [Authorize(Roles = "admin")]
        public IActionResult ManageBookings()
        {
            var booking = _obrsContext.tbl_bookings
                .Include(b => b.bus)
                    .ThenInclude(bus => bus.routes)
                .Include(b => b.bus.price) // ✅ Price bhi include karo
                .ToList();

            return View(booking);
        }

        [Authorize(Roles = "admin")]
        public IActionResult UpdateBookings(Guid id)
        {
            var userbooking = _obrsContext.tbl_bus
                .Include(b => b.routes)  // 👈 make sure you included route in Bus model
                .Select(b => new
                {
                    BusId = b.BusId,
                    DisplayName = b.BusNumber + " - " + b.routes.RouteName
                })
                .ToList();

            ViewBag.Empbuses = new SelectList(userbooking, "BusId", "DisplayName");
            var booking = _obrsContext.tbl_bookings.Find(id);
            return View(booking);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult UpdateBookings(Booking B)
        {
            var booking = _obrsContext.tbl_bookings.FirstOrDefault(x => x.BookingId == B.BookingId);
            if (booking == null) return NotFound();

            // Admin can only update Status
            booking.Status = B.Status;
            booking.bus = B.bus;

            _obrsContext.SaveChanges();

            TempData["update"] = "Booking status has been updated successfully";
            return RedirectToAction("ManageBookings", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult DeleteBookings(Guid id)
        {
            var booking = _obrsContext.tbl_bookings.Find(id);
            _obrsContext.tbl_bookings.Remove(booking);
            _obrsContext.SaveChanges();
            TempData["danger"] = "Booking has been deleted successfully";
            return RedirectToAction("ManageBookings", "Admin");
        }

        // End booking CRUD
        [Authorize(Roles = "admin")]
        // Start User CRUD
        public async Task<IActionResult> ManageUsers()
        {
            var users = oBRSContext.Users.ToList();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "N/A";
            }

            ViewBag.UserRoles = userRoles; // pass roles dictionary to view

            return View(users);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult ToggleUser(string id)   // ⚡ now it's string (IdentityUser primary key is string, not Guid)
        {
            var user = oBRSContext.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                TempData["danger"] = "⚠️ User not found.";
                return RedirectToAction("ManageUsers", "Admin");
            }

            // Toggle logic
            user.IsActive = !user.IsActive;

            oBRSContext.Users.Update(user);
            oBRSContext.SaveChanges();

            if (user.IsActive)
                TempData["success"] = $"{user.FullName} is now Active.";
            else
                TempData["warning"] = $"{user.FullName} is now Inactive.";

            return RedirectToAction("ManageUsers", "Admin");
        }
        [Authorize(Roles = "admin")]
        public IActionResult DeleteUsers(string id)   // ⚡ must be string, not Guid
        {
            var user = oBRSContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                TempData["danger"] = "⚠️ User not found.";
                return RedirectToAction("ManageUsers", "Admin");
            }

            oBRSContext.Users.Remove(user);
            oBRSContext.SaveChanges();

            TempData["danger"] = "User has been deleted successfully.";
            return RedirectToAction("ManageUsers", "Admin");
        }

        // End user CRUD

        [Authorize(Roles = "admin")]
        // ✅ Manage Contact Messages
        public IActionResult ManageContacts()
        {
            var contacts = _obrsContext.tbl_contact
                                       .OrderByDescending(c => c.CreatedAt)
                                       .ToList();
            return View(contacts);
        }

        [Authorize(Roles = "admin")]
        public IActionResult DetailsContact(int id)
        {
            var contact = _obrsContext.tbl_contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                TempData["danger"] = "⚠️ Message not found.";
                return RedirectToAction("ManageContacts", "Admin");
            }

            return View(contact);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteContact(int id)
        {
            var contact = _obrsContext.tbl_contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                TempData["danger"] = "⚠️ Message not found.";
                return RedirectToAction("ManageContacts", "Admin");
            }

            _obrsContext.tbl_contact.Remove(contact);
            _obrsContext.SaveChanges();

            TempData["danger"] = "Message deleted successfully.";
            return RedirectToAction("ManageContacts", "Admin");
        }

    }
}
