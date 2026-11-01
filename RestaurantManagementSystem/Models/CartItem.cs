namespace RestaurantManagementSystem.Models
{
    public class CartItem
    {
        public FoodItem Food { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal
        {
            get { return Food.Price * Quantity; }
        }

        public CartItem(FoodItem food, int quantity)
        {
            Food = food;
            Quantity = quantity;
        }
    }
}
