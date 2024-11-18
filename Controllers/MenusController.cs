using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantService.Models;
using System.Net.Http;

namespace RestaurantService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public MenusController(RestaurantContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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

        [HttpPut("updateAvailability/{menuId}")]
        public async Task<IActionResult> UpdateAvailability(int menuId, [FromBody] bool availability)
        {
            var menu = await _context.Menus.FindAsync(menuId);
            if (menu == null)
            {
                return NotFound();
            }

            menu.Availability = availability;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("syncAvailability")]
        public async Task<IActionResult> SyncAvailability()
        {
            // Fetch availability updates from the order service
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://order-service/api/availability");
            if (response.IsSuccessStatusCode)
            {
                var availabilityUpdates = JsonConvert.DeserializeObject<List<AvailabilityUpdate>>(await response.Content.ReadAsStringAsync());
                foreach (var update in availabilityUpdates)
                {
                    var menu = await _context.Menus.FindAsync(update.MenuId);
                    if (menu != null)
                    {
                        menu.Availability = update.Availability;
                    }
                }
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }

    public class AvailabilityUpdate
    {
        public int MenuId { get; set; }
        public bool Availability { get; set; }
    }

}