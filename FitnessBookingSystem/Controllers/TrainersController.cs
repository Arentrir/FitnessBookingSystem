using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessBookingSystem.Data;
using FitnessBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessBookingSystem.Controllers
{
    // Всеки трябва да може да гледа Index и Details, затова махаме глобалното [Authorize]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        // Публично достъпен
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchString, string sortOrder, int? pageNumber)
        {
            // Текущи филтри и сортиране
            ViewData["CurrentSort"] = sortOrder;
            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewData["LastNameSortParm"] = sortOrder == "last" ? "last_desc" : "last";
            ViewData["ExperienceSortParm"] = sortOrder == "exp" ? "exp_desc" : "exp";
            ViewData["RatingSortParm"] = sortOrder == "rating" ? "rating_desc" : "rating";
            ViewData["SpecializationSortParm"] = sortOrder == "spec" ? "spec_desc" : "spec";
            ViewData["CurrentFilter"] = searchString;

            var trainers = _context.Trainers.AsQueryable();

            // Търсене
            if (!string.IsNullOrEmpty(searchString))
            {
                trainers = trainers.Where(t =>
                    t.FirstName.Contains(searchString) ||
                    t.LastName.Contains(searchString));
            }

            // Сортиране
            trainers = sortOrder switch
            {
                "first_desc" => trainers.OrderByDescending(t => t.FirstName),
                "last" => trainers.OrderBy(t => t.LastName),
                "last_desc" => trainers.OrderByDescending(t => t.LastName),
                "exp" => trainers.OrderBy(t => t.ExperienceYears),
                "exp_desc" => trainers.OrderByDescending(t => t.ExperienceYears),
                "rating" => trainers.OrderBy(t => t.Rating),
                "rating_desc" => trainers.OrderByDescending(t => t.Rating),
                "spec" => trainers.OrderBy(t => t.Specialization),
                "spec_desc" => trainers.OrderByDescending(t => t.Specialization),
                _ => trainers.OrderBy(t => t.FirstName)
            };

            int pageSize = 10;
            return View(await PaginatedList<Trainer>.CreateAsync(trainers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }




        // GET: Trainers/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // GET: Trainers/Create
        // Само Admin може да създава треньори
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                // 1. Ако има снимка – качваме я
                if (photo != null && photo.Length > 0)
                {
                    var uploadFolder = Path.Combine("wwwroot", "uploads", "trainers");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    trainer.PhotoPath = "/uploads/trainers/" + fileName;
                }

                trainer.CreatedAt = DateTime.Now;

                _context.Add(trainer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }


        // GET: Trainers/Edit/5
        // Само Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Trainer trainer, IFormFile Photo)
        {
            if (id != trainer.TrainerId)
                return NotFound();

            var existing = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.TrainerId == id);
            if (existing == null)
                return NotFound();

            // Запазваме CreatedAt
            trainer.CreatedAt = existing.CreatedAt;

            // Ако има нова снимка
            if (Photo != null && Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads/trainers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }

                trainer.PhotoPath = "/uploads/trainers/" + fileName;
            }
            else
            {
                trainer.PhotoPath = existing.PhotoPath;
            }

            _context.Update(trainer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Trainers/Delete/5
        // Само Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer != null)
                _context.Trainers.Remove(trainer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }
    }
}

