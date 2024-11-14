using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantService.Models;


namespace RestaurantService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public RestaurantsController(RestaurantContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.Include(r => r.Menus).FirstOrDefaultAsync(r => r.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // Fetch owner details from the user service
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://user-service/api/users/{restaurant.OwnerId}");
            if (response.IsSuccessStatusCode)
            {
                var owner = JsonConvert.DeserializeObject<Owner>(await response.Content.ReadAsStringAsync());
                restaurant.Owner = owner;
            }

            return restaurant;
        }

        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            // Send a notification
            var client = _httpClientFactory.CreateClient();
            var notification = new Notification
            {
                Message = $"New restaurant added: {restaurant.Name}",
                Recipient = "subscribers@example.com"
            };
            var content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
            await client.PostAsync("https://notification-service/api/notifications", content);

            return CreatedAtAction("GetRestaurant", new { id = restaurant.RestaurantId }, restaurant);
        }
    }
}

