using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using HotcakesAdminPro;
using Newtonsoft.Json.Linq;

namespace HotcakesAdminPro.Tests
{
    [TestFixture]
    public class EmptyStateTests
    {
        [Test]
        public void CalculateWeeklySummary_WithEmptyList_ReturnsZeroValues()
        {
            // Arrange - Üres JSON tömb létrehozása
            var service = new StatService();
            var emptyOrders = new JArray();
            var testDate = DateTime.UtcNow.AddDays(-7);

            // Act - Számítás futtatása
            var result = service.CalculateWeeklySummary(emptyOrders, testDate);

            // Assert - Ellenőrizzük, hogy nem omlott össze és 0 az eredmény
            Assert.AreEqual(0, result.Revenue, "Üres lista esetén a bevételnek 0-nak kell lennie.");
            Assert.AreEqual(0, result.CustomerCount, "Üres lista esetén a vásárlók számának 0-nak kell lennie.");
        }
    }
}
