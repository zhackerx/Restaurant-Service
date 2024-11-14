namespace RestaurantService.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<Menu> Menus { get; set; }
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
    }
}
