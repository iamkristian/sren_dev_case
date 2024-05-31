using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BriefingService.Data;
using BriefingService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BriefingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BriefingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BriefingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Briefings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Briefing>>> GetBriefings()
        {
            return await _context.Briefings.ToListAsync();
        }

        // GET: api/Briefings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Briefing>> GetBriefing(int id)
        {
            var briefing = await _context.Briefings.FindAsync(id);

            if (briefing == null)
            {
                return NotFound();
            }

            return briefing;
        }

        // POST: api/Briefings
        [HttpPost]
        public async Task<ActionResult<Briefing>> PostBriefing(Briefing briefing)
        {
            Console.WriteLine("This is a log");
            _context.Briefings.Add(briefing);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBriefing", new { id = briefing.Id }, briefing);
        }

        // PUT: api/Briefings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBriefing(int id, Briefing briefing)
        {
            if (id != briefing.Id)
            {
                return BadRequest();
            }

            _context.Entry(briefing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BriefingExists(id))
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

        // DELETE: api/Briefings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBriefing(int id)
        {
            var briefing = await _context.Briefings.FindAsync(id);
            if (briefing == null)
            {
                return NotFound();
            }

            _context.Briefings.Remove(briefing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BriefingExists(int id)
        {
            return _context.Briefings.Any(e => e.Id == id);
        }
    }
}
