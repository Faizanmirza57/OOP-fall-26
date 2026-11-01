using System;
using System.Collections.Generic;

namespace RestaurantManagementSystem.Models
{
    public class RestaurantOrder
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalTotal { get; set; }

        public RestaurantOrder(string orderNumber, string customerName,
            DateTime orderDate, List<CartItem> items, decimal subtotal,
            decimal discount, decimal finalTotal)
        {
            OrderNumber = orderNumber;
            CustomerName = customerName;
            OrderDate = orderDate;
            Items = items;
            Subtotal = subtotal;
            Discount = discount;
            FinalTotal = finalTotal;
        }
    }
}
