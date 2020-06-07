using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSWEB_project.Models;
using RSWEB_project.ViewModels;

namespace RSWEB_project.Controllers
{
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TeacherController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // Index
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
           var teachers = from m in _context.Teacher
                          select m;

            teachers = teachers.Include(t => t.Courses1).Include(t => t.Courses2);

            ViewData["FNameSort"] = String.IsNullOrEmpty(sortOrder) ? "FNameD" : "";
            ViewData["LNameSort"] = String.IsNullOrEmpty(sortOrder) ? "LNameD" : "";
            ViewData["DegreeSort"] = sortOrder=="Degree" ? "DegreeD" : "Degree";
            ViewData["RankSort"] = sortOrder == "Rank" ? "RankD" : "Rank";
            ViewData["NumSort"] = sortOrder == "Number" ? "NumberD" : "Number";
            ViewData["DateSort"] = sortOrder == "HireDate" ? "HireDateD" : "HireDate";

            teachers = sortOrder switch
            {
                "Degree" => teachers.OrderBy(t => t.Degree),
                "DegreeD" => teachers.OrderByDescending(t => t.Degree),
                "Rank" => teachers.OrderBy(t => t.AcademicRank),
                "RankD" => teachers.OrderByDescending(t => t.AcademicRank),
                "Number" => teachers.OrderBy(t => t.OfficeNumber),
                "NumberD" => teachers.OrderByDescending(t => t.OfficeNumber),
                "HireDate" => teachers.OrderBy(t => t.HireDate),
                "HireDateD" => teachers.OrderByDescending(t => t.HireDate),
                "FNameD" => teachers.OrderByDescending(t => t.FirstName),
                "LNameD" => teachers.OrderByDescending(t => t.LastName),
                _ => teachers.OrderBy(t => t.FirstName),
            };

            if (!String.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(s => s.FirstName.Contains(searchString) || s.LastName.Contains(searchString) || s.AcademicRank.Contains(searchString) || s.Degree.Contains(searchString));
            }
          
            return View(await teachers.ToListAsync());
        }

        // Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.Include(t=>t.Courses1).Include(t=>t.Courses2)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // Create
        public IActionResult Create()
        {
            return View();
        }

        // Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                if(ViewModel.ProfileImage != null)
                {
                    string uniqueFileName = UploadedFile(ViewModel);
                    ViewModel.Teacher.ProfilePicture = uniqueFileName;
                }

                else
                {
                    ViewModel.Teacher.ProfilePicture = "_default.png";
                }

                _context.Add(ViewModel.Teacher);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ViewModel);
        }

        // Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherViewModel ViewModel = new TeacherViewModel
            {
                Teacher = teacher,
                ProfileImage = null
            };

            return View(ViewModel);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel ViewModel)
        {
            if (id != ViewModel.Teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(ViewModel.ProfileImage != null)
                    {
                        string uniqueFileName = UploadedFile(ViewModel);
                        ViewModel.Teacher.ProfilePicture = uniqueFileName;
                    }

                    _context.Update(ViewModel.Teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(ViewModel.Teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(ViewModel);
        }

        // Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        public async Task<IActionResult> TeacherLogin()
        {
            var teachers = from m in _context.Teacher
                           select m;

            return View(await teachers.ToListAsync());
        }

        private string UploadedFile(TeacherViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
