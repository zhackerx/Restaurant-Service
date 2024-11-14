namespace RestaurantService.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public int RestaurantId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public Restaurant Restaurant { get; set; }
    }

}
