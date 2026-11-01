using System;
using System.Collections.Generic;
using System.IO;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem
{
    internal class Program
    {
        private static RestaurantService _service;

        private static void Main(string[] args)
        {
            string dataFolder = Directory.GetCurrentDirectory();
            MenuRepository menuRepository = new MenuRepository(
                Path.Combine(dataFolder, "menu.txt"));
            OrderRepository orderRepository = new OrderRepository(
                Path.Combine(dataFolder, "orders.txt"));

            _service = new RestaurantService(menuRepository, orderRepository);
            MainMenu();
        }

        private static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n====================================");
                Console.WriteLine("       RESTAURANT MANAGEMENT SYSTEM");
                Console.WriteLine("====================================");
                Console.WriteLine("1. Admin Panel");
                Console.WriteLine("2. Customer Panel");
                Console.WriteLine("3. Exit");

                int choice = ReadInt("Enter choice: ", 1);

                if (choice == 1)
                {
                    User admin = AdminLogin();

                    if (admin != null)
                    {
                        Console.WriteLine("Welcome, " + admin.Name + " (" + admin.Role + ")!");
                        AdminPanel();
                    }
                }
                else if (choice == 2)
                {
                    CustomerPanel();
                }
                else if (choice == 3)
                {
                    Console.WriteLine("Thank you for using the system!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select 1, 2 or 3.");
                }
            }
        }

        private static AdminUser AdminLogin()
        {
            Console.WriteLine("\n========== ADMIN LOGIN ==========");

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                string username = ReadText("Enter username: ");
                string password = ReadText("Enter password: ");

                if (username == "admin" && password == "1234")
                {
                    Console.WriteLine("Login successful!");
                    return new AdminUser(username);
                }

                Console.WriteLine("Wrong username or password. Attempts left: " + (3 - attempt));
            }

            Console.WriteLine("Login failed. Returning to main menu.");
            return null;
        }

        private static void AdminPanel()
        {
            while (true)
            {
                Console.WriteLine("\n========== ADMIN PANEL ==========");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. Add Food");
                Console.WriteLine("3. Update Food");
                Console.WriteLine("4. Delete Food");
                Console.WriteLine("5. Search Food");
                Console.WriteLine("6. View Order History");
                Console.WriteLine("7. Logout");

                int choice = ReadInt("Enter choice: ", 1);

                switch (choice)
                {
                    case 1:
                        DisplayMenu(_service.GetMenu());
                        break;
                    case 2:
                        AddFood();
                        break;
                    case 3:
                        UpdateFood();
                        break;
                    case 4:
                        DeleteFood();
                        break;
                    case 5:
                        SearchFood();
                        break;
                    case 6:
                        ViewOrderHistory();
                        break;
                    case 7:
                        Console.WriteLine("Logged out successfully.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void CustomerPanel()
        {
            while (true)
            {
                Console.WriteLine("\n========== CUSTOMER PANEL ==========");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. Search Food");
                Console.WriteLine("3. Place Order");
                Console.WriteLine("4. Back to Main Menu");

                int choice = ReadInt("Enter choice: ", 1);

                switch (choice)
                {
                    case 1:
                        DisplayMenu(_service.GetMenu());
                        break;
                    case 2:
                        SearchFood();
                        break;
                    case 3:
                        PlaceOrder();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void DisplayMenu(List<FoodItem> menu)
        {
            Console.WriteLine("\n================ RESTAURANT MENU ================");

            if (menu.Count == 0)
            {
                Console.WriteLine("No food items available.");
                return;
            }

            Console.WriteLine("{0,-25}{1,-15}{2,-10}", "Food", "Price (Rs.)", "Stock");
            Console.WriteLine("--------------------------------------------------");

            foreach (FoodItem item in menu)
            {
                Console.WriteLine("{0,-25}{1,-15:0.00}{2,-10}",
                    item.Name, item.Price, item.Stock);
            }
        }

        private static void AddFood()
        {
            Console.WriteLine("\n========== ADD FOOD ==========");
            string name = ReadText("Enter food name: ");
            decimal price = ReadDecimal("Enter price: ", 0.01m);
            int stock = ReadInt("Enter stock quantity: ", 0);

            string message;
            _service.AddFood(new FoodItem(name, price, stock), out message);
            Console.WriteLine(message);
        }

        private static void UpdateFood()
        {
            DisplayMenu(_service.GetMenu());
            string name = ReadText("Enter the exact food name to update: ");
            decimal price = ReadDecimal("Enter new price: ", 0.01m);
            int stock = ReadInt("Enter new stock quantity: ", 0);

            string message;
            _service.UpdateFood(name, price, stock, out message);
            Console.WriteLine(message);
        }

        private static void DeleteFood()
        {
            DisplayMenu(_service.GetMenu());
            string name = ReadText("Enter the exact food name to delete: ");

            if (ReadYesNo("Are you sure you want to delete this item? (y/n): "))
            {
                if (_service.DeleteFood(name))
                {
                    Console.WriteLine("Food item deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Food item not found.");
                }
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        private static void SearchFood()
        {
            string searchText = ReadText("Enter food name or keyword: ");
            List<FoodItem> results = _service.SearchFood(searchText);

            if (results.Count == 0)
            {
                Console.WriteLine("No matching food items found.");
            }
            else
            {
                DisplayMenu(results);
            }
        }

        private static void ViewOrderHistory()
        {
            List<string> orders = _service.GetOrderHistory();

            Console.WriteLine("\n========== ORDER HISTORY ==========");

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders have been placed yet.");
                return;
            }

            foreach (string order in orders)
            {
                Console.WriteLine(order);
            }
        }

        private static void PlaceOrder()
        {
            if (_service.GetMenu().Count == 0)
            {
                Console.WriteLine("No food items are available right now.");
                return;
            }

            Console.WriteLine("\n========== PLACE ORDER ==========");
            User customer = new Customer(ReadText("Enter your name: "));
            Console.WriteLine("Welcome, " + customer.Name + " (" + customer.Role + ").");
            List<CartItem> cart = new List<CartItem>();

            while (true)
            {
                DisplayMenu(_service.GetMenu());
                string foodName = ReadText("Enter food name: ");
                int quantity = ReadInt("Enter quantity: ", 1);

                string message;
                bool added = _service.AddToCart(cart, foodName, quantity, out message);
                Console.WriteLine(message);

                if (added)
                {
                    DisplayCart(cart);
                }

                if (!ReadYesNo("Do you want to add another item? (y/n): "))
                {
                    break;
                }
            }

            if (cart.Count == 0)
            {
                Console.WriteLine("No order placed.");
                return;
            }

            Console.WriteLine("\n========== ORDER SUMMARY ==========");
            DisplayCart(cart);

            if (!ReadYesNo("Confirm and place this order? (y/n): "))
            {
                Console.WriteLine("Order cancelled. No stock was changed.");
                return;
            }

            RestaurantOrder order = _service.PlaceOrder(customer.Name, cart);

            if (order == null)
            {
                Console.WriteLine("Unable to place order. Please check item stock and try again.");
                return;
            }

            PrintReceipt(order);
        }

        private static void DisplayCart(List<CartItem> cart)
        {
            decimal total = 0;
            Console.WriteLine("\nYour Cart:");

            foreach (CartItem item in cart)
            {
                Console.WriteLine(item.Food.Name + " x " + item.Quantity +
                    " = Rs. " + item.LineTotal.ToString("0.00"));
                total += item.LineTotal;
            }

            Console.WriteLine("Current Subtotal: Rs. " + total.ToString("0.00"));
        }

        private static void PrintReceipt(RestaurantOrder order)
        {
            Console.WriteLine("\n====================================");
            Console.WriteLine("              FINAL BILL");
            Console.WriteLine("====================================");
            Console.WriteLine("Order Number:  " + order.OrderNumber);
            Console.WriteLine("Date:          " + order.OrderDate.ToString("yyyy-MM-dd HH:mm"));
            Console.WriteLine("Customer Name: " + order.CustomerName);
            Console.WriteLine("------------------------------------");

            foreach (CartItem item in order.Items)
            {
                Console.WriteLine(item.Food.Name + " x " + item.Quantity +
                    " = Rs. " + item.LineTotal.ToString("0.00"));
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Subtotal:      Rs. " + order.Subtotal.ToString("0.00"));
            Console.WriteLine("Discount:      Rs. " + order.Discount.ToString("0.00"));
            Console.WriteLine("Final Bill:    Rs. " + order.FinalTotal.ToString("0.00"));
            Console.WriteLine("====================================");
            Console.WriteLine("Order placed successfully!");
        }

        private static int ReadInt(string prompt, int minimumValue)
        {
            while (true)
            {
                Console.Write(prompt);
                int value;

                if (int.TryParse(Console.ReadLine(), out value) && value >= minimumValue)
                {
                    return value;
                }

                Console.WriteLine("Please enter a whole number equal to or greater than " + minimumValue + ".");
            }
        }

        private static decimal ReadDecimal(string prompt, decimal minimumValue)
        {
            while (true)
            {
                Console.Write(prompt);
                decimal value;

                if (decimal.TryParse(Console.ReadLine(), out value) && value >= minimumValue)
                {
                    return value;
                }

                Console.WriteLine("Please enter a valid amount greater than zero.");
            }
        }

        private static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(value) && !value.Contains("|"))
                {
                    return value.Trim();
                }

                Console.WriteLine("Please enter a valid value. The | character is not allowed.");
            }
        }

        private static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string answer = Console.ReadLine();

                if (answer != null && (answer.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    answer.Equals("yes", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }

                if (answer != null && (answer.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                    answer.Equals("no", StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                Console.WriteLine("Please enter y or n.");
            }
        }
    }
}
