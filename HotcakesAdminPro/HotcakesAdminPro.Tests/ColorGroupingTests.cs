using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using HotcakesAdminPro;

namespace HotcakesAdminPro.Tests
{
    [TestFixture]
    public class ColorGroupingTests
    {
        [Test]
        public void GetGroupedColorStats_CaseSensitivity_GroupsAllUnderOneCategory()
        {
            // Arrange - Előkészítjük a teszt adatokat (3 Pink, 2 pink, 1 PINK)
            var service = new StatService();
            var rawData = new List<(string Color, int Quantity)>
            {
                ("Pink", 3),
                ("pink", 2),
                ("PINK", 1),
                ("Zöld", 5)
            };

            // Act - Futtatjuk a csoportosítást
            var result = service.GetGroupedColorStats(rawData);

            // Assert - Ellenőrizzük az eredményt
            // 1. Csak 2 kategóriánk legyen (Pink és Zöld), ne 4!
            Assert.AreEqual(2, result.Count, "A különböző írásmódú színeket nem vonta össze a rendszer!");

            // 2. A Pink kategóriában pontosan 6 terméknek kell lennie
            Assert.IsTrue(result.ContainsKey("Pink"));
            Assert.AreEqual(6, result["Pink"], "A Pink darabszámok összeadása hibás!");
        }
    }
}
