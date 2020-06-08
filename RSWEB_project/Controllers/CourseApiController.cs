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
    public class CourseApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CourseApiController(AppDbContext context)
        {
            _context = context;
        }

        //GET: api/CourseApi
        [HttpGet]
        public IEnumerable<Course> Get()
        {
            return _context.Course;
        }

        //GET: api/CourseApi/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
        // PUT: api/CourseApi/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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


        // POST: api/CourseApi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = course.Id }, course);
        }

        // DELETE: api/CourseApi/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        private bool CourseExists(long id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}