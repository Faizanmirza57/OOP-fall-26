using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data
{
    public class OrderRepository
    {
        private readonly string _filePath;

        public OrderRepository(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveOrder(RestaurantOrder order)
        {
            StringBuilder line = new StringBuilder();
            line.Append("Order: ").Append(order.OrderNumber);
            line.Append(" | Date: ").Append(order.OrderDate.ToString("yyyy-MM-dd HH:mm"));
            line.Append(" | Customer: ").Append(order.CustomerName.Replace("|", "/"));
            line.Append(" | Items: ");

            for (int i = 0; i < order.Items.Count; i++)
            {
                CartItem item = order.Items[i];

                if (i > 0)
                {
                    line.Append(", ");
                }

                line.Append(item.Food.Name).Append(" x ").Append(item.Quantity);
            }

            line.Append(" | Subtotal: Rs. ")
                .Append(order.Subtotal.ToString("0.00", CultureInfo.InvariantCulture));
            line.Append(" | Discount: Rs. ")
                .Append(order.Discount.ToString("0.00", CultureInfo.InvariantCulture));
            line.Append(" | Final: Rs. ")
                .Append(order.FinalTotal.ToString("0.00", CultureInfo.InvariantCulture));

            File.AppendAllText(_filePath, line + System.Environment.NewLine);
        }

        public List<string> LoadOrders()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            return new List<string>(File.ReadAllLines(_filePath));
        }
    }
}
