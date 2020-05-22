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
    public class TeacherController : Controller
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
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
            return View(teacher);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
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
    }
}
