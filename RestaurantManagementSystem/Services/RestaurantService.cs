using System;
using System.Collections.Generic;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services
{
    public class RestaurantService
    {
        private readonly MenuRepository _menuRepository;
        private readonly OrderRepository _orderRepository;
        private readonly List<FoodItem> _menu;

        public RestaurantService(MenuRepository menuRepository, OrderRepository orderRepository)
        {
            _menuRepository = menuRepository;
            _orderRepository = orderRepository;
            _menu = _menuRepository.LoadMenu();

            if (!_menuRepository.FileExists)
            {
                AddSampleMenu();
                _menuRepository.SaveMenu(_menu);
            }
        }

        private void AddSampleMenu()
        {
            _menu.Add(new FoodItem("Chicken Burger", 650, 20));
            _menu.Add(new FoodItem("Zinger Burger", 850, 15));
            _menu.Add(new FoodItem("Pizza Slice", 450, 25));
            _menu.Add(new FoodItem("French Fries", 300, 30));
            _menu.Add(new FoodItem("Cold Drink", 150, 40));
        }

        public List<FoodItem> GetMenu()
        {
            return _menu;
        }

        public FoodItem FindFood(string name)
        {
            foreach (FoodItem item in _menu)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        public List<FoodItem> SearchFood(string searchText)
        {
            List<FoodItem> results = new List<FoodItem>();

            foreach (FoodItem item in _menu)
            {
                if (item.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public bool AddFood(FoodItem newItem, out string message)
        {
            if (newItem.Price <= 0 || newItem.Stock < 0)
            {
                message = "Price must be greater than zero and stock cannot be negative.";
                return false;
            }

            if (FindFood(newItem.Name) != null)
            {
                message = "A food item with this name already exists.";
                return false;
            }

            _menu.Add(newItem);
            _menuRepository.SaveMenu(_menu);
            message = "Food item added successfully.";
            return true;
        }

        public bool UpdateFood(string name, decimal newPrice, int newStock, out string message)
        {
            FoodItem item = FindFood(name);

            if (item == null)
            {
                message = "Food item not found.";
                return false;
            }

            if (newPrice <= 0 || newStock < 0)
            {
                message = "Price must be greater than zero and stock cannot be negative.";
                return false;
            }

            item.Price = newPrice;
            item.Stock = newStock;
            _menuRepository.SaveMenu(_menu);
            message = "Food item updated successfully.";
            return true;
        }

        public bool DeleteFood(string name)
        {
            FoodItem item = FindFood(name);

            if (item == null)
            {
                return false;
            }

            _menu.Remove(item);
            _menuRepository.SaveMenu(_menu);
            return true;
        }

        public bool AddToCart(List<CartItem> cart, string foodName, int quantity, out string message)
        {
            if (quantity <= 0)
            {
                message = "Quantity must be greater than zero.";
                return false;
            }

            FoodItem food = FindFood(foodName);

            if (food == null)
            {
                message = "Food item not found.";
                return false;
            }

            CartItem existingCartItem = null;

            foreach (CartItem cartItem in cart)
            {
                if (cartItem.Food == food)
                {
                    existingCartItem = cartItem;
                    break;
                }
            }

            int alreadyInCart = existingCartItem == null ? 0 : existingCartItem.Quantity;
            int availableStock = food.Stock - alreadyInCart;

            if (quantity > availableStock)
            {
                message = "Not enough stock. Available quantity: " + availableStock;
                return false;
            }

            if (existingCartItem == null)
            {
                cart.Add(new CartItem(food, quantity));
            }
            else
            {
                existingCartItem.Quantity += quantity;
            }

            message = food.Name + " added to cart.";
            return true;
        }

        public RestaurantOrder PlaceOrder(string customerName, List<CartItem> cart)
        {
            if (cart.Count == 0)
            {
                return null;
            }

            decimal subtotal = 0;

            foreach (CartItem item in cart)
            {
                if (item.Quantity <= 0 || item.Quantity > item.Food.Stock)
                {
                    return null;
                }

                subtotal += item.LineTotal;
            }

            decimal discount = 0;

            if (subtotal >= 5000)
            {
                discount = subtotal * 15 / 100;
            }
            else if (subtotal >= 3000)
            {
                discount = subtotal * 10 / 100;
            }
            else if (subtotal >= 1500)
            {
                discount = subtotal * 5 / 100;
            }

            decimal finalTotal = subtotal - discount;
            List<CartItem> orderItems = new List<CartItem>();

            foreach (CartItem item in cart)
            {
                orderItems.Add(new CartItem(item.Food, item.Quantity));
            }

            RestaurantOrder order = new RestaurantOrder(
                DateTime.Now.ToString("yyyyMMddHHmmss"), customerName,
                DateTime.Now, orderItems, subtotal, discount, finalTotal);

            foreach (CartItem item in cart)
            {
                item.Food.Stock -= item.Quantity;
            }

            _menuRepository.SaveMenu(_menu);
            _orderRepository.SaveOrder(order);
            return order;
        }

        public List<string> GetOrderHistory()
        {
            return _orderRepository.LoadOrders();
        }
    }
}
