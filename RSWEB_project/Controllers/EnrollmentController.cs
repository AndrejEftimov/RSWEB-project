using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RSWEB_project.Models;
using RSWEB_project.ViewModels;

namespace RSWEB_project.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public EnrollmentController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // Index (IMPLEMENT SORTING)
        public async Task<IActionResult> Index()
        {
            var onlineCoursesContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);

            return View(await onlineCoursesContext.ToListAsync());
        }

        // Details/id
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // Create
        public IActionResult Create()
        {
            ViewData["Courseid"] = new SelectList(_context.Course, "Id", "Title");
            ViewData["Studentid"] = new SelectList(_context.Student, "Id", "FullName");

            return View();
        }

        // Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Courseid"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["Studentid"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);

            return View(enrollment);
        }

        // Edit/id
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["Courseid"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["Studentid"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);

            return View(enrollment);
        }

        // Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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

            ViewData["Courseid"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["Studentid"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);

            return View(enrollment);
        }

        // Delete/id
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EnrollmentExists (private method)
        private bool EnrollmentExists(long id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }

        // TeacherCourseIndex
        [HttpGet]
        public async Task<IActionResult> TeacherCourseIndex([FromQuery] int CourseId, [FromQuery] string Year, [FromQuery] int TeacherId)
        {
            var enrollments = _context.Enrollment.Include(e => e.Course).Include(e => e.Student).AsQueryable();

            string year;

            if (!String.IsNullOrEmpty(Year))
            {
                year = Year;
            }

            else
            {
                year = DateTime.Now.Year.ToString();
            }

            enrollments = enrollments.Where(e => e.Course.Id.Equals(CourseId) && e.Year.ToString().Equals(year));
            enrollments = enrollments.OrderBy(e => e.Student.FirstName);

            ViewData["CourseId"] = CourseId;
            ViewData["TeacherId"] = TeacherId;

            // for the _NonAdminLayout
            Teacher t = await _context.Teacher.FindAsync(TeacherId);
            ViewData["ProfilePicture"] = t.ProfilePicture;
            ViewData["FullName"] = t.FullName;

            return View(await enrollments.ToListAsync());
        }

        // TeacherCourseEdit
        public async Task<IActionResult> TeacherCourseEdit(long? id, int TeacherId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            ViewData["TeacherId"] = TeacherId; // for previous page

            // for the _NonAdminLayout
            Teacher t = await _context.Teacher.FindAsync(TeacherId);
            ViewData["ProfilePicture"] = t.ProfilePicture;
            ViewData["FullName"] = t.FullName;

            return View(enrollment);
        }

        // TeacherCourseEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherCourseEdit(long Id, int TeacherId, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (Id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("TeacherCourseIndex", new { CourseId = enrollment.CourseId, TeacherId = TeacherId });
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            ViewData["TeacherId"] = TeacherId; // for previous page

            // for the _NonAdminLayout
            Teacher t = await _context.Teacher.FindAsync(TeacherId);
            ViewData["ProfilePicture"] = t.ProfilePicture;
            ViewData["FullName"] = t.FullName;

            return View(enrollment);
        }

        // StudentIndex
        public async Task<IActionResult> StudentIndex(long StudentId)
        {
            var enrollments = _context.Enrollment.Include(e => e.Course).Include(e => e.Student).AsQueryable();

            enrollments = enrollments.Where(e => e.StudentId.Equals(StudentId));
            enrollments = enrollments.OrderByDescending(e => e.Semester);

            // for the _NonAdminLayout
            Student s = await _context.Student.FindAsync(StudentId);
            ViewData["ProfilePicture"] = s.ProfilePicture;
            ViewData["FullName"] = s.FullName;

            return View(await enrollments.ToListAsync());
        }

        // StudentEdit
        public async Task<IActionResult> StudentEdit(long? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(Id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);

            // for the _NonAdminLayout
            Student s = await _context.Student.FindAsync(enrollment.StudentId);
            ViewData["ProfilePicture"] = s.ProfilePicture;
            ViewData["FullName"] = s.FullName;

            EnrollmentUploadViewModel ViewModel = new EnrollmentUploadViewModel
            {
                Enrollment = enrollment,
                SeminalFile = null
            };

            return View(ViewModel);
        }

        // StudentEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentEdit(long Id, EnrollmentUploadViewModel ViewModel)
        {
            if (Id != ViewModel.Enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(ViewModel);
                    ViewModel.Enrollment.SeminalUrl = uniqueFileName;

                    _context.Update(ViewModel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("StudentIndex", new { StudentId = ViewModel.Enrollment.StudentId });
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", ViewModel.Enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", ViewModel.Enrollment.StudentId);

            return View(ViewModel);
        }

        // AdminCreate
        public IActionResult AdminCreate()
        {
            ViewData["CourseIds"] = new SelectList(_context.Course, "Id", "Title");
            ViewData["StudentIds"] = new SelectList(_context.Student, "Id", "FullName");

            return View();
        }

        // AdminCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate([Bind("Year,Semester,CourseId,StudentIds")] EnrollmentsViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                foreach (int id in viewmodel.StudentIds)
                {
                    Enrollment e = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.StudentId == id && e.CourseId == viewmodel.CourseId &&
                        e.Year == viewmodel.Year && e.Semester == viewmodel.Semester);

                    if (e == null)
                    {
                        e = new Enrollment
                        {
                            StudentId = id,
                            CourseId = viewmodel.CourseId,
                            Year = viewmodel.Year,
                            Semester = viewmodel.Semester
                        };

                        _context.Add(e);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseIds"] = new SelectList(_context.Course, "Id", "Title", viewmodel.CourseId);
            ViewData["StudentIds"] = new SelectList(_context.Student, "Id", "FullName", viewmodel.StudentIds);

            return View(viewmodel);
        }

        // AdminEdit/id
        public async Task<IActionResult> AdminEdit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // AdminEdit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(long id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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

            return View(enrollment);
        }

        // AdminEditMultiple
        public async Task<IActionResult> AdminEditMultiple(int? courseId, int? year)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollment;
            List<Course> courses = await _context.Course.ToListAsync();

            if (courseId != null && year != null)
            {
                enrollments = enrollments.Where(e => e.CourseId == courseId);
                enrollments = enrollments.Where(e => e.Year == year);
                enrollments = enrollments.Include(e => e.Student);

                ViewData["Enrollments"] = new SelectList(enrollments.ToList().OrderBy(e => e.Student.FullName), "Id", "Student.FullName");
            }

            else
            {
                enrollments = null;
            }

            UnenrollMultipleViewModel ViewModel = new UnenrollMultipleViewModel
            {
                CourseId = courseId,
                Year = year
            };

            ViewData["Courses"] = new SelectList(courses.OrderBy(c => c.Title), "Id", "Title");

            return View(ViewModel);
        }

        // AdminEditMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEditMultiple(UnenrollMultipleViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (int Id in ViewModel.EnrollmentIds)
                {
                    Enrollment enrollment = await _context.Enrollment
                        .FirstOrDefaultAsync(e => e.Id == Id);

                    if (enrollment != null)
                    {
                        enrollment.FinishDate = ViewModel.FinishDate;
                        _context.Update(enrollment);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("AdminEditMultiple");
        }

        private string UploadedFile(EnrollmentUploadViewModel model)
        {
            string uniqueFileName = null;

            if (model.SeminalFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "documents");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.SeminalFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SeminalFile.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
