using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSWEB_project.Models;

namespace RSWEB_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TeacherApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> Get()
        {
            return await _context.Teacher.ToListAsync();
        }

        // GET: api/TeacherApi/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> Get(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // PUT: api/TeacherApi/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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

        // POST: api/TeacherApi
        [HttpPost]
        public async Task<ActionResult<Teacher>> Post(Teacher teacher)
        {
            _context.Teacher.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = teacher.Id }, teacher);
        }

        // DELETE: api/TeacherApi/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Teacher>> Delete(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
