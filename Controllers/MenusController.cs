using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Models;

namespace RestaurantService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public MenusController(RestaurantContext context)
        {
            _context = context;
        }

        [HttpGet("{restaurantId}")]
        public async Task<ActionResult<IEnumerable<Menu>>> GetMenus(int restaurantId)
        {
            var menus = await _context.Menus.Where(m => m.RestaurantId == restaurantId).ToListAsync();

            if (menus == null || !menus.Any())
            {
                return NotFound();
            }

            return menus;
        }

        [HttpPost]
        public async Task<ActionResult<Menu>> PostMenu(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMenu", new { id = menu.MenuId }, menu);
        }
    }

}
