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
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public StudentController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // Index
        public async Task<IActionResult> Index(string courseString, string searchString, string sortOrder)
        {
            var students = from m in _context.Student
                           select m;

            students = students.Include(s => s.Enrollments).ThenInclude(s => s.Course);

            //on every call, set one field & reset all other fields.
            ViewData["FNameSort"] = String.IsNullOrEmpty(sortOrder) ? "FNameD" : "";
            ViewData["CreditsSort"] = sortOrder == "Credits" ? "CreditsD" : "Credits";
            ViewData["LNameSort"] = String.IsNullOrEmpty(sortOrder) ? "LNameD" : "";
            ViewData["StudentIdSort"] = sortOrder == "StudentId" ? "StudentIdD" : "StudentId";
            ViewData["DateSort"] = sortOrder == "Date" ? "DateD" : "Date";
            ViewData["SemSort"] = sortOrder == "Semester" ? "SemesterD" : "Semester";
            ViewData["LevelSort"] = sortOrder == "EducationLevel" ? "EducationLevelD" : "EducationLevel";

            students = sortOrder switch
            {
                "FNameD" => students.OrderByDescending(s => s.FirstName),
                "Credits" => students.OrderBy(s => s.AcquiredCredits),
                "CreditsD" => students.OrderByDescending(s => s.AcquiredCredits),
                "StudentId" => students.OrderBy(s => s.StudentId),
                "StudentIdD" => students.OrderByDescending(s => s.StudentId),
                "Date" => students.OrderBy(s => s.EnrollmentDate),
                "DateD" => students.OrderByDescending(s => s.EnrollmentDate),
                "Semester" => students.OrderBy(s => s.CurrentSemester),
                "SemesterD" => students.OrderByDescending(s => s.CurrentSemester),
                "LNameD" => students.OrderByDescending(s => s.LastName),
                "EducationLevel" => students.OrderBy(s => s.EducationLevel),
                "EducationLevelD" => students.OrderByDescending(s => s.EducationLevel),
                _ => students.OrderBy(s => s.FirstName),
            };

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.StudentId.Contains(searchString) || s.FirstName.Contains(searchString) || s.LastName.Contains(searchString));

            }

            if (!String.IsNullOrEmpty(courseString))
            {
                var enrollments = _context.Enrollment.AsQueryable();
                enrollments = enrollments.Where(m => m.Course.Title.Contains(courseString));
                students = students.Where(s => s.Enrollments.Any(e => e.Course.Title == courseString));

            }

            return View(await students.ToListAsync());
        }

        // Details
        public async Task<IActionResult> Details(long? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.Include(s => s.Enrollments).ThenInclude(s => s.Course)
                // .Include(s => s.Enrollments.Where(e => e.Studentid == id))
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        // Create
        public IActionResult Create()
        {
            return View();
        }

        // Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                if(ViewModel.ProfileImage != null)
                {
                    string uniqueFileName = UploadedFile(ViewModel);
                    ViewModel.Student.ProfilePicture = uniqueFileName;
                }

                else
                {
                    ViewModel.Student.ProfilePicture = "_default.png";
                }

                _context.Add(ViewModel.Student);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ViewModel);
        }

        // Edit/id
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            StudentViewModel ViewModel = new StudentViewModel
            {
                Student = student,
                ProfileImage = null
            };

            return View(ViewModel);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, StudentViewModel ViewModel)
        {
            if (id != ViewModel.Student.Id)
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
                        ViewModel.Student.ProfilePicture = uniqueFileName;
                    }

                    _context.Update(ViewModel.Student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(ViewModel.Student.Id))
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
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var student = await _context.Student.FindAsync(id);

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return _context.Student.Any(e => e.Id == id);
        }

        public async Task<IActionResult> StudentLogin()
        {
            var students = from m in _context.Student
                           select m;

            return View(await students.ToListAsync());
        }

        private string UploadedFile(StudentViewModel model)
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
