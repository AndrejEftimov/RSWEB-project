using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RSWEB_project.Models;

namespace RSWEB_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EnrollmentApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> Get()
        {
            return await _context.Enrollment.ToListAsync();
        }

        // GET: api/EnrollmentApi/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> Get(long id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // PUT: api/EnrollmentApi/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return BadRequest();
            }

            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EnrollmentApi
        [HttpPost]
        public async Task<ActionResult<Enrollment>> Post(Enrollment enrollment)
        {
            _context.Enrollment.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = enrollment.Id }, enrollment);
        }

        // DELETE: api/EnrollmentApi/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Enrollment>> Delete(long id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();

            return enrollment;
        }

        private bool EnrollmentExists(long id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
