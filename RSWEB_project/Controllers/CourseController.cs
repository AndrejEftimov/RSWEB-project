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
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        // Index
        public async Task<IActionResult> Index(string teacherString, string searchString, string sortOrder)
        {
            var courses = from c in _context.Course
                          select c;

            courses = courses.Include(c => c.Teacher2).Include(c => c.Teacher1).Include(c => c.Enrollments).ThenInclude(c => c.Student);

            ViewData["TitleSort"] = String.IsNullOrEmpty(sortOrder) ? "TitleD" : "";
            ViewData["CreditsSort"] = sortOrder == "Credits" ? "CreditsD" : "Credits";
            ViewData["SemesterSort"] = sortOrder == "Semester" ? "SemesterD" : "Semester";
            ViewData["ProgrammeSort"] = sortOrder == "Programme" ? "ProgrammeD" : "Programme";
            ViewData["LevelSort"] = sortOrder == "EducationLevel" ? "EducationLevelD" : "EducationLevel";
            ViewData["FTidSort"] = sortOrder == "FTid" ? "FTidD" : "FTid";
            ViewData["STidSort"] = sortOrder == "STid" ? "STidD" : "STid";

            courses = sortOrder switch
            {
                "Credits" => courses.OrderBy(s => s.Credits),
                "CreditsD" => courses.OrderByDescending(s => s.Credits),
                "Semester" => courses.OrderBy(s => s.Semester),
                "SemesterD" => courses.OrderByDescending(s => s.Semester),
                "Programme" => courses.OrderBy(s => s.Programme),
                "ProgrammeD" => courses.OrderByDescending(s => s.Programme),
                "EducationLevel" => courses.OrderBy(s => s.EducationLevel),
                "EducationLevelD" => courses.OrderByDescending(s => s.EducationLevel),
                "FTid" => courses.OrderBy(s => s.FirstTeacherId),
                "FTidD" => courses.OrderByDescending(s => s.FirstTeacherId),
                "STid" => courses.OrderBy(s => s.SecondTeacherId),
                "STidD" => courses.OrderByDescending(s => s.SecondTeacherId),
                "TitleD" => courses.OrderByDescending(s => s.Title),
                _ => courses.OrderBy(c => c.Title),
            };

            if (!String.IsNullOrEmpty(searchString))
            {
                int x = 0;
                Int32.TryParse(searchString, out x);
                courses = courses.Where(s => s.Title.Contains(searchString) || s.Semester == x || s.Programme.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(teacherString))
            {
                // var  teachers = _context.Teacher.Where(m=>m.FirstName.Contains(teacherString));

                int y = 0;
                Int32.TryParse(teacherString, out y);
                courses = courses.Where(s => s.FirstTeacherId == y || s.SecondTeacherId == y || s.Teacher1.FirstName.Contains(teacherString) || s.Teacher2.FirstName.Contains(teacherString));

            }

            return View(await courses.ToListAsync());
        }

        // Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.Include(t => t.Teacher1).Include(t => t.Teacher2).Include(c => c.Enrollments).ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // Create
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName");
            return View();
        }

        // Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.Include(c => c.Teacher1).Include(c => c.Teacher2)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
