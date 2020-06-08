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
    public class StudentApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StudentApiController(AppDbContext context)
        {
            _context = context;
        }

        //GET: api/StudentApi
        [HttpGet]
        public IEnumerable<Student> GetAll()
        {
            return _context.Student;
        }

        //GET: api/StudentApi/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/StudentApi/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/StudentApi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }

        // DELETE: api/StudentApi/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        private bool StudentExists(long id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}