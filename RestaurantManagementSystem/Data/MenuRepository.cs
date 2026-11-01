using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data
{
    public class MenuRepository
    {
        private readonly string _filePath;

        public bool FileExists
        {
            get { return File.Exists(_filePath); }
        }

        public MenuRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<FoodItem> LoadMenu()
        {
            List<FoodItem> items = new List<FoodItem>();

            if (!File.Exists(_filePath))
            {
                return items;
            }

            string[] lines = File.ReadAllLines(_filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts;

                if (line.Contains("|"))
                {
                    parts = line.Split('|');
                }
                else
                {
                    // This also reads the old C++ menu format.
                    parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                }

                if (parts.Length != 3)
                {
                    continue;
                }

                decimal price;
                int stock;

                if (decimal.TryParse(parts[1], NumberStyles.Number,
                    CultureInfo.InvariantCulture, out price) &&
                    int.TryParse(parts[2], out stock) &&
                    price > 0 && stock >= 0)
                {
                    items.Add(new FoodItem(parts[0].Trim(), price, stock));
                }
            }

            return items;
        }

        public void SaveMenu(List<FoodItem> items)
        {
            List<string> lines = new List<string>();

            foreach (FoodItem item in items)
            {
                string name = item.Name.Replace("|", "/");
                string line = name + "|" +
                    item.Price.ToString(CultureInfo.InvariantCulture) + "|" +
                    item.Stock;
                lines.Add(line);
            }

            File.WriteAllLines(_filePath, lines);
        }
    }
}
