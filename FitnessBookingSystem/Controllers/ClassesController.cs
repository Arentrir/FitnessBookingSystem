using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessBookingSystem.Data;
using FitnessBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessBookingSystem.Controllers
{
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Classes
        // Публично достъпно
        [AllowAnonymous]
        public async Task<IActionResult> Index(

         string? sortOrder,
         string? currentFilter,
         string? searchString,
         int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["StartTimeSortParm"] = sortOrder == "StartTime" ? "starttime_desc" : "StartTime";
            ViewData["DurationSortParm"] = sortOrder == "Duration" ? "duration_desc" : "Duration";
            ViewData["CapacitySortParm"] = sortOrder == "Capacity" ? "capacity_desc" : "Capacity";
            ViewData["TrainerSortParm"] = sortOrder == "Trainer" ? "trainer_desc" : "Trainer";
            ViewData["CreatedAtSortParm"] = sortOrder == "CreatedAt" ? "createdat_desc" : "CreatedAt";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var classes = _context.Classes.Include(c => c.Trainer).AsQueryable();

            // Ако потребителят е Trainer, показваме само неговите класове
            if (User.IsInRole("Trainer"))
            {
                var username = User.Identity.Name;
                classes = classes.Where(c => c.CreatedBy == username);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(c =>
                    c.Title.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            // Сортиране
            classes = sortOrder switch
            {
                "title_desc" => classes.OrderByDescending(c => c.Title),
                "StartTime" => classes.OrderBy(c => c.StartTime),
                "starttime_desc" => classes.OrderByDescending(c => c.StartTime),
                "Duration" => classes.OrderBy(c => c.DurationMinutes),
                "duration_desc" => classes.OrderByDescending(c => c.DurationMinutes),
                "Capacity" => classes.OrderBy(c => c.Capacity),
                "capacity_desc" => classes.OrderByDescending(c => c.Capacity),
                "Trainer" => classes.OrderBy(c => c.Trainer.FirstName),
                "trainer_desc" => classes.OrderByDescending(c => c.Trainer.FirstName),
                "CreatedAt" => classes.OrderBy(c => c.CreatedAt),
                "createdat_desc" => classes.OrderByDescending(c => c.CreatedAt),
                _ => classes.OrderBy(c => c.Title),
            };

            int pageSize = 10;
            return View(await PaginatedList<Class>.CreateAsync(classes.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: Classes/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var classEntity = await _context.Classes
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(m => m.ClassId == id);

            if (classEntity == null)
                return NotFound();

            return View(classEntity);
        }

        // GET: Classes/Create
        [Authorize(Roles = "Admin,Trainer")]
        public IActionResult Create()
        {
            // Взимаме всички треньори
            var trainers = _context.Trainers.ToList();
            ViewData["TrainerId"] = new SelectList(trainers, "TrainerId", "FirstName");
            return View();
        }


        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Create(Class classObj)
        {
            if (ModelState.IsValid)
            {
                classObj.CreatedAt = DateTime.Now;
                classObj.CreatedBy = User.Identity.Name;

                _context.Add(classObj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Ако ModelState не е валиден, върни SelectList
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "FirstName", classObj.TrainerId);
            return View(classObj);
        }

        // GET: Classes/Edit/5
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var classObj = await _context.Classes.FindAsync(id);
            if (classObj == null) return NotFound();

            // Ограничение
            if (!User.IsInRole("Admin") && classObj.CreatedBy != User.Identity.Name)
                return Forbid(); // 403 Forbidden

            return View(classObj);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Edit(int id, Class classObj)
        {
            if (id != classObj.ClassId) return NotFound();

            var existing = await _context.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.ClassId == id);
            if (existing == null) return NotFound();

            if (!User.IsInRole("Admin") && existing.CreatedBy != User.Identity.Name)
                return Forbid();

            classObj.CreatedBy = existing.CreatedBy; // Не сменяме създателя
            classObj.CreatedAt = existing.CreatedAt; // Не сменяме датата

            _context.Update(classObj);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Classes/Delete/5
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var classObj = await _context.Classes.FindAsync(id);
            if (classObj == null) return NotFound();

            if (!User.IsInRole("Admin") && classObj.CreatedBy != User.Identity.Name)
                return Forbid();

            return View(classObj);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classObj = await _context.Classes.FindAsync(id);
            if (!User.IsInRole("Admin") && classObj.CreatedBy != User.Identity.Name)
                return Forbid();

            _context.Classes.Remove(classObj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }
    }
}
