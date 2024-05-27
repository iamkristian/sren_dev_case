using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderListService.Data;
using OrderListService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderListService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderList>>> GetOrderLists()
        {
            return await _context.OrderLists.Include(o => o.Assets).ToListAsync();
        }

        // GET: api/OrderLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderList>> GetOrderList(int id)
        {
            var orderList = await _context.OrderLists.Include(o => o.Assets).FirstOrDefaultAsync(o => o.Id == id);

            if (orderList == null)
            {
                return NotFound();
            }

            return orderList;
        }

        // POST: api/OrderLists
        [HttpPost]
        public async Task<ActionResult<OrderList>> PostOrderList(OrderList orderList)
        {
            _context.OrderLists.Add(orderList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderList", new { id = orderList.Id }, orderList);
        }

        // PUT: api/OrderLists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderList(int id, OrderList orderList)
        {
            if (id != orderList.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderListExists(id))
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

        // DELETE: api/OrderLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderList(int id)
        {
            var orderList = await _context.OrderLists.FindAsync(id);
            if (orderList == null)
            {
                return NotFound();
            }

            _context.OrderLists.Remove(orderList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderListExists(int id)
        {
            return _context.OrderLists.Any(e => e.Id == id);
        }
    }
}
