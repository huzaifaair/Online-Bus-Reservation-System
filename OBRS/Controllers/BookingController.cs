using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBRS.Areas.Identity.Data;
using OBRS.Data;
using OBRS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OBRS.Controllers
{
    public class BookingController : Controller
    {
        // 🔹 Admin DB context (buses, bookings, etc.)
        private readonly obrsContext _bookingContext;

        // 🔹 Identity DB context (AspNetUsers)
        private readonly OBRSContext _identityContext;

        private readonly UserManager<OBRSUser> _userManager;

        public BookingController(obrsContext bookingContext, UserManager<OBRSUser> userManager, OBRSContext identityContext)
        {
            _bookingContext = bookingContext;
            _userManager = userManager;
            _identityContext = identityContext;
        }

        // ------------------ GET: Create Booking ------------------
        [HttpGet]
        public IActionResult Create(Guid busId)
        {
            var bus = _bookingContext.tbl_bus
                        .Include(b => b.routes)
                        .Include(b => b.price)
                        .FirstOrDefault(b => b.BusId == busId);

            if (bus == null) return NotFound();

            var bookedSeats = _bookingContext.tbl_bookings
                                .Where(b => b.Bus_id == busId && b.Status == "Booked")
                                .Select(b => b.SeatNumber)
                                .ToList();

            ViewBag.Bus = bus;
            ViewBag.BookedSeats = bookedSeats;

            return View(new Booking
            {
                Bus_id = busId,
                TravelDate = DateTime.Today
            });
        }

        // ------------------ POST: Create Booking ------------------
        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            var currentUserId = _userManager.GetUserId(User);
            Console.WriteLine("👉 CurrentUserId: " + (currentUserId ?? "NULL"));

            if (string.IsNullOrEmpty(currentUserId))
            {
                Console.WriteLine("❌ User is not logged in.");
                ModelState.AddModelError("", "You must be logged in to book a seat.");
                LoadBusAndSeats(booking.Bus_id);
                return View(booking);
            }

            // Identity context (AspNetUsers) check
            var exists = _identityContext.Users.Any(u => u.Id == currentUserId);
            Console.WriteLine("👉 Exists in AspNetUsers? " + exists);

            if (!exists)
            {
                Console.WriteLine("❌ UserId not found in AspNetUsers table.");
                ModelState.AddModelError("", "Invalid user. Please re-login.");
                LoadBusAndSeats(booking.Bus_id);
                return View(booking);
            }

            // Seat check (Booking context)
            bool seatTaken = _bookingContext.tbl_bookings.Any(b =>
                b.Bus_id == booking.Bus_id &&
                b.SeatNumber == booking.SeatNumber &&
                b.Status == "Booked" &&
                b.TravelDate.Date == booking.TravelDate.Date
            );
            Console.WriteLine("👉 SeatTaken? " + seatTaken);

            if (seatTaken)
            {
                ModelState.AddModelError("SeatNumber", "This seat is already booked.");
                LoadBusAndSeats(booking.Bus_id);
                return View(booking);
            }

            // Booking create karo
            booking.BookingId = Guid.NewGuid();
            booking.Status = "Booked";
            booking.BookingDate = DateTime.Now;
            booking.UserId = currentUserId;

            Console.WriteLine($"✅ New Booking about to save. BusId: {booking.Bus_id}, Seat: {booking.SeatNumber}");

            _bookingContext.tbl_bookings.Add(booking);
            _bookingContext.SaveChanges();

            Console.WriteLine("🎉 Booking saved successfully. Redirecting to Confirmation...");

            return RedirectToAction("Confirmation", "Booking", new { id = booking.BookingId });
        }

        [HttpPost]
        public IActionResult TestPost()
        {
            return Content("✅ Test POST hit hua");
        }

        // ------------------ GET: My Bookings ------------------
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);

            var bookings = await _bookingContext.tbl_bookings
                .Include(b => b.bus)
                    .ThenInclude(bus => bus.routes)
                .Include(b => b.bus.price)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // ------------------ GET: Booking Confirmation ------------------
        [HttpGet]
        public IActionResult Confirmation(Guid id)
        {
            var booking = _bookingContext.tbl_bookings
                            .Include(b => b.bus)
                                .ThenInclude(bus => bus.routes)
                            .Include(b => b.bus.price)
                            .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // ------------------ Private Helper ------------------
        private void LoadBusAndSeats(Guid busId)
        {
            var bus = _bookingContext.tbl_bus
                        .Include(b => b.routes)
                        .Include(b => b.price)
                        .FirstOrDefault(b => b.BusId == busId);

            var bookedSeats = _bookingContext.tbl_bookings
                                .Where(b => b.Bus_id == busId && b.Status == "Booked")
                                .Select(b => b.SeatNumber)
                                .ToList();

            ViewBag.Bus = bus;
            ViewBag.BookedSeats = bookedSeats;
        }
    }
}
