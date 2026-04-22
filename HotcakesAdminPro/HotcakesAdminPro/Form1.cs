using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace HotcakesAdminPro
{
    public partial class Form1 : Form
    {
        // --- API BEÁLLÍTÁSOK ---
        private const string BaseUrl = "http://nerokozmetika.hu/DesktopModules/Hotcakes/API/rest/v1/";
        private const string ApiKey = "1-3518c8a8-8059-47bf-9918-5dc030e1ad34";
        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            SetupVipGrid(); // Táblázat inicializálása induláskor
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ezt teljesen üresen hagyjuk!
            // Csak azért van itt, hogy a Visual Studio tervezője megnyugodjon.
        }

        // --- TÁBLÁZAT DESIGN ÉS OSZLOPOK ---
        private void SetupVipGrid()
        {
            gridVip.Columns.Clear();
            gridVip.Columns.Add("Email", "Vásárló E-mail címe");
            gridVip.Columns.Add("OrderCount", "Rendelések száma");
            gridVip.Columns.Add("TotalSpent", "Összesen elköltve (Ft)");

            gridVip.Columns["Email"].Width = 250;
            gridVip.Columns["TotalSpent"].DefaultCellStyle.Format = "N0"; // Ezres tagoló a szép árakért
            gridVip.AllowUserToAddRows = false;
            gridVip.ReadOnly = true;
            gridVip.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridVip.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // --- KÖZÖS API HÍVÓ FÜGGVÉNY ---
        private async Task<string> GetApiDataAsync(string endpoint)
        {
            string url = $"{BaseUrl}{endpoint}?key={ApiKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // --- VIP GOMB KATTINTÁS (Itt pótoltam az 'async' szót!) ---
        private async void btnLoadVip_Click_1(object sender, EventArgs e)
        {
            try
            {
                btnLoadVip.Enabled = false;
                btnLoadVip.Text = "Kalkuláció folyamatban...";
                gridVip.Rows.Clear(); // Előző keresés törlése

                // 1. Rendelések lekérése a "GET /orders" végpontról
                string ordersJson = await GetApiDataAsync("orders");
                                
                JObject ordersData = JObject.Parse(ordersJson);

                // Szótár: Email -> (Rendelések száma, Elköltött összeg)
                Dictionary<string, Tuple<int, decimal>> vasarloiStatisztika = new Dictionary<string, Tuple<int, decimal>>();

                // 2. JSON feldolgozása
                foreach (var order in ordersData["Content"])
                {
                    string email = order["UserEmail"]?.ToString();
                    if (string.IsNullOrEmpty(email)) continue; // Ha nincs email, kihagyjuk

                    // Végösszeg kinyerése (Hotcakes TotalGrand mező)
                    decimal orderTotal = 0;
                    if (order["TotalGrand"] != null)
                    {
                        decimal.TryParse(order["TotalGrand"].ToString(), out orderTotal);
                    }

                    // 3. Adatok aggregálása (összeadása)
                    if (vasarloiStatisztika.ContainsKey(email))
                    {
                        int regiDb = vasarloiStatisztika[email].Item1;
                        decimal regiOsszeg = vasarloiStatisztika[email].Item2;
                        vasarloiStatisztika[email] = new Tuple<int, decimal>(regiDb + 1, regiOsszeg + orderTotal);
                    }
                    else
                    {
                        vasarloiStatisztika[email] = new Tuple<int, decimal>(1, orderTotal);
                    }
                }

                // 4. Top 10 Vásárló sorba rendezése összeg szerint csökkenőbe
                var topVasarlok = vasarloiStatisztika
                    .OrderByDescending(x => x.Value.Item2)
                    .Take(10);

                // 5. Kiíratás a táblázatba
                foreach (var vasarlo in topVasarlok)
                {
                    gridVip.Rows.Add(vasarlo.Key, vasarlo.Value.Item1, vasarlo.Value.Item2);
                }

                btnLoadVip.Text = "Sikeres frissítés!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a VIP keresés közben:\n{ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLoadVip.Text = "VIP Vásárlók Keresése";
            }
            finally
            {
                btnLoadVip.Enabled = true; // Gomb visszakapcsolása
            }
        }
    }
}