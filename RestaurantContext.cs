using Microsoft.EntityFrameworkCore;
using RestaurantService.Models;
using System.Collections.Generic;

namespace RestaurantService
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options) { }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Owner> Owners { get; set; }
    }

}
