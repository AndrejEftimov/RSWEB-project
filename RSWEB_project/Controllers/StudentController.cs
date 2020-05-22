using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RSWEB_project.Models;

namespace RSWEB_project.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
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
                "Semester" => students.OrderBy(s => s.CurrentSemestar),
                "SemesterD" => students.OrderByDescending(s => s.CurrentSemestar),
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
            var enrols = _context.Enrollment;
            var student = await _context.Student.Include(s=>s.Enrollments).ThenInclude(s=>s.Course)
               // .Include(s => s.Enrollments.Where(e=>e.Studentid == id))
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
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
            return View(student);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
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
    }
}
