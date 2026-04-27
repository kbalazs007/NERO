using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using HotcakesAdminPro; // Ez biztosítja, hogy látja a StatService-t

namespace HotcakesAdminPro.Tests
{
    [TestFixture]
    public class TrendTests
    {
        private StatService _statService;

        [SetUp]
        public void Setup()
        {
            // Minden teszt előtt létrehozunk egy friss példányt
            _statService = new StatService();
        }

        [Test]
        [Description("Ellenőrizzük, hogy a trend számítása pontos-e különböző adatokkal.")]
        [TestCase(150, 100, 50.0)]   // 50%-os növekedés
        [TestCase(33, 100, -67.0)]   // 67%-os visszaesés (a képernyőmentésed alapján!)
        [TestCase(100, 100, 0.0)]    // Nincs változás
        public void CalculateTrend_Scenarios_ReturnsExpectedPercentage(int current, int prev, double expected)
        {
            // Act - Meghívjuk a kiszervezett logikát
            double result = _statService.CalculateTrend(current, prev);

            // Assert - Ellenőrizzük az eredményt (0.1-es tűréshatárral a kerekítés miatt)
            Assert.AreEqual(expected, result, 0.1, $"Hiba a számításnál: {current} vs {prev}");
        }

        [Test]
        [Description("Kritikus teszt: Nem omlik-e össze a program, ha az előző havi adat 0?")]
        public void CalculateTrend_WhenPreviousIsZero_ReturnsZero()
        {
            // Act
            double result = _statService.CalculateTrend(50, 0);

            // Assert
            Assert.AreEqual(0, result, "Nullával való osztás esetén 0-át várunk hiba helyett.");
        }

        [Test]
        [Description("Ellenőrizzük, hogy a kijelzett szöveg formátuma megfelel-e az elvárásnak.")]
        public void FormatTrendText_PositiveValue_ReturnsCorrectString()
        {
            // Act
            string result = _statService.FormatTrendText(15);

            // Assert
            // A kódod szerinti formátum: " (Trend: +15% vs előző hónap)" 
            Assert.That(result, Does.Contain("+15%"));
            Assert.That(result, Does.StartWith(" (Trend:"));
        }
    }
}
