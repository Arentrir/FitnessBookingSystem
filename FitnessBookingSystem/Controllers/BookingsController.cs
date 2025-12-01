using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessBookingSystem.Data;
using FitnessBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace FitnessBookingSystem.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;


        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ClassSortParm"] = String.IsNullOrEmpty(sortOrder) ? "class_desc" : "";
            ViewData["UserSortParm"] = sortOrder == "user" ? "user_desc" : "user";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var bookings = _context.Bookings.Include(b => b.Class).AsQueryable();

            // Търсене
            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => b.Class.Title.Contains(searchString) || b.UserEmail.Contains(searchString));
            }

            // Сортиране
            bookings = sortOrder switch
            {
                "class_desc" => bookings.OrderByDescending(b => b.Class.Title),
                "user" => bookings.OrderBy(b => b.UserEmail),
                "user_desc" => bookings.OrderByDescending(b => b.UserEmail),
                "date" => bookings.OrderBy(b => b.BookingDate),
                "date_desc" => bookings.OrderByDescending(b => b.BookingDate),
                _ => bookings.OrderBy(b => b.Class.Title),
            };

            // Странициране
            int pageSize = 10;
            return View(await PaginatedList<Booking>.CreateAsync(bookings.AsNoTracking(), pageNumber ?? 1, pageSize));
        }






        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Class)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        [Authorize(Roles = "User,Admin")]
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Title");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Title", booking.ClassId);
                return View(booking);
            }

            // Настройваме датата на резервацията
            booking.BookingDate = DateTime.Now;

            // Взимаме класа и взимаме StartTime
            var selectedClass = await _context.Classes.FindAsync(booking.ClassId);
            if (selectedClass == null)
            {
                ModelState.AddModelError("ClassId", "Selected class does not exist.");
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Title", booking.ClassId);
                return View(booking);
            }

            // CreatedAt = StartTime на класа
            booking.CreatedAt = selectedClass.StartTime;

            // По подразбиране резервацията не е одобрена
            booking.IsApproved = false;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Bookings/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Title", booking.ClassId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Title", booking.ClassId);
                return View(booking);
            }

            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Class)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
