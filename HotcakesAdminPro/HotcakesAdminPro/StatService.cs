using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotcakesAdminPro
{
    public class StatService
    {
        public double CalculateTrend(int currentPeriodOrders, int previousPeriodOrders)
        {
            if (previousPeriodOrders <= 0) return 0;
            return ((double)(currentPeriodOrders - previousPeriodOrders) / previousPeriodOrders) * 100;
        }

        public string FormatTrendText(double diff)
        {
            if (diff == 0) return "";
            return $" (Trend: {diff:+0;-0}% vs előző hónap)";
        }

        public Dictionary<string, int> GetGroupedColorStats(IEnumerable<(string Color, int Quantity)> rawEntries)
        {
            // A StringComparer.OrdinalIgnoreCase biztosítja, hogy a "Pink" és a "pink" ugyanaz a kulcs legyen
            var stats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in rawEntries)
            {
                if (string.IsNullOrWhiteSpace(entry.Color)) continue;

                if (!stats.ContainsKey(entry.Color))
                {
                    stats[entry.Color] = 0;
                }
                stats[entry.Color] += entry.Quantity;
            }

            return stats;
        }

        public (decimal Revenue, int CustomerCount) CalculateWeeklySummary(JArray orders, DateTime since)
        {
            decimal totalRevenue = 0;
            var uniqueCustomers = new HashSet<string>();

            if (orders == null) return (0, 0);

            foreach (var order in orders.OfType<JObject>())
            {
                DateTime orderDate = order["TimeOfOrderUtc"] != null
                    ? (DateTime)order["TimeOfOrderUtc"]
                    : DateTime.MinValue;

                if (orderDate >= since)
                {
                    decimal total = 0;
                    decimal.TryParse(order["TotalGrand"]?.ToString(), out total);
                    totalRevenue += total;

                    string email = order["UserEmail"]?.ToString();
                    if (!string.IsNullOrEmpty(email))
                    {
                        uniqueCustomers.Add(email);
                    }
                }
            }
            return (totalRevenue, uniqueCustomers.Count);
        }

    }
}
