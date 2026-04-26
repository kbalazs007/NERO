using System;
using System.Collections.Generic;
using System.IO; // CSV olvasáshoz
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace HotcakesAdminPro
{
    public partial class Form1 : Form
    {
        // --- API ÉS FÁJL BEÁLLÍTÁSOK ---
        private const string BaseUrl = "http://nerokozmetika.hu/DesktopModules/Hotcakes/API/rest/v1/";
        private const string ApiKey = "1-3518c8a8-8059-47bf-9918-5dc030e1ad34";
        private static readonly HttpClient client = new HttpClient();

        // Ez a szótár tárolja a Python által kinyert SKU -> Szín párokat
        private Dictionary<string, string> SkuColorMap = new Dictionary<string, string>();
        private Chart chartColors;

        public Form1()
        {
            InitializeComponent();
            LoadSkuColors(); // CSV betöltése induláskor
            SetupVipGrid();
            SetupChart();

            btnLoadColors.Click += btnLoadColors_Click;
        }

        // --- CSV BETÖLTÉSE ---
        private void LoadSkuColors()
        {
            try
            {
                // A fájlnak a .exe mellett kell lennie (vagy adj meg teljes útvonalat)
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "C:\\Users\\kacsa\\Documents\\Corvinus\\4. félév\\Rendszerfejlesztés és IT projektmanagement (projekttárgy)\\Kliensalkalmazás\\v3\\HotcakesAdminPro\\sku_colors.csv");

                if (!File.Exists(path))
                {
                    MessageBox.Show("A 'sku_colors.csv' nem található a program mappájában! A statisztika nem fog működni.", "Hiányzó adatfájl");
                    return;
                }

                SkuColorMap.Clear();
                var lines = File.ReadAllLines(path);

                // Fejléc kihagyása (Skip 1), majd feldolgozás
                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        string sku = parts[0].Trim();
                        string color = parts[1].Trim();
                        SkuColorMap[sku] = color;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a CSV betöltése közben: " + ex.Message);
            }
        }

        private void SetupChart()
        {
            chartColors = new Chart();
            chartColors.Dock = DockStyle.Fill;
            ChartArea chartArea = new ChartArea("MainArea");
            chartColors.ChartAreas.Add(chartArea);
            tabPage1.Controls.Add(chartColors);

            chartColors.Series.Clear();
            Series series = new Series("Színek");
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            // Megjelenítjük a színt és a darabszámot is a címkén
            series.Label = "#VALX: #VAL db";
            chartColors.Series.Add(series);

            chartColors.Titles.Clear();
            chartColors.Titles.Add("Színstatisztika az SKU adatbázis alapján");
        }

        // --- SZÍNSTATISZTIKA GENERÁLÁSA ---
        private async void btnLoadColors_Click(object sender, EventArgs e)
        {
            try
            {
                btnLoadColors.Enabled = false;
                btnLoadColors.Text = "Elemzés folyamatban...";
                chartColors.Series["Színek"].Points.Clear();

                if (SkuColorMap.Count == 0)
                {
                    MessageBox.Show("Nincsenek betöltött SKU adatok a CSV-ből!");
                    return;
                }

                // 1. Rendelések lekérése
                string ordersSummaryJson = await GetApiDataAsync("orders");
                JObject ordersSummaryData = JObject.Parse(ordersSummaryJson);

                Dictionary<string, int> szinStatisztika = new Dictionary<string, int>();

                // Gyorsítótár: Hogy ne kérjük le ugyanazt a terméket százszor az API-tól
                Dictionary<string, string> letoltottTermekSkuCache = new Dictionary<string, string>();

                if (ordersSummaryData["Content"] is JArray ordersArray)
                {
                    foreach (JObject orderSummary in ordersArray.OfType<JObject>())
                    {
                        decimal total = 0;
                        if (orderSummary["TotalGrand"] != null) decimal.TryParse(orderSummary["TotalGrand"].ToString(), out total);

                        // Csak a valódi vásárlásokat nézzük
                        if (total > 0)
                        {
                            string orderBvin = orderSummary["bvin"]?.ToString() ?? orderSummary["Bvin"]?.ToString();
                            string singleOrderJson = await GetApiDataAsync($"orders/{orderBvin}");
                            JObject fullOrder = (JObject)JObject.Parse(singleOrderJson)["Content"];

                            JArray items = (fullOrder["Items"] as JArray) ?? (fullOrder["LineItems"] as JArray);
                            if (items != null)
                            {
                                foreach (JObject item in items.OfType<JObject>())
                                {
                                    string productId = item["ProductId"]?.ToString();
                                    int qty = 0;
                                    if (item["Quantity"] != null) int.TryParse(item["Quantity"].ToString(), out qty);

                                    if (!string.IsNullOrEmpty(productId) && qty > 0)
                                    {
                                        string valodiSku = "";

                                        // MÉLYFÚRÁS: Benne van már a memóriánkban ez a termék?
                                        if (letoltottTermekSkuCache.ContainsKey(productId))
                                        {
                                            valodiSku = letoltottTermekSkuCache[productId];
                                        }
                                        else
                                        {
                                            // Ha nincs, rárúgjuk az ajtót a katalógus API-ra a tényleges SKU-ért!
                                            string singleProductJson = await GetApiDataAsync($"products/{productId}");
                                            JObject productData = JObject.Parse(singleProductJson);
                                            JObject fullProduct = productData["Content"] as JObject;

                                            valodiSku = fullProduct?["Sku"]?.ToString() ?? "";

                                            // Eltesszük a memóriába, hogy legközelebb már ne kelljen letölteni
                                            letoltottTermekSkuCache[productId] = valodiSku;
                                        }

                                        // DÖNTÉSI LOGIKA: Szerepel a VALÓDI SKU a CSV listában?
                                        if (!string.IsNullOrEmpty(valodiSku) && SkuColorMap.ContainsKey(valodiSku))
                                        {
                                            string color = SkuColorMap[valodiSku];

                                            if (!szinStatisztika.ContainsKey(color))
                                                szinStatisztika[color] = 0;

                                            szinStatisztika[color] += qty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // 2. Diagram frissítése
                if (szinStatisztika.Count > 0)
                {
                    foreach (var stat in szinStatisztika.OrderByDescending(x => x.Value))
                    {
                        chartColors.Series["Színek"].Points.AddXY(stat.Key, stat.Value);
                    }
                    btnLoadColors.Text = "Sikeres frissítés!";
                }
                else
                {
                    MessageBox.Show("Sikeresen letöltöttük a valódi SKU-kat, de egyik sem egyezik a CSV-ben lévő adatokkal.", "Nincs egyezés");
                    btnLoadColors.Text = "Frissítés (Nincs adat)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba:\n{ex.Message}");
                btnLoadColors.Text = "Hiba történt";
            }
            finally
            {
                btnLoadColors.Enabled = true;
            }
        }

        // --- KÖZÖS API HÍVÓ FÜGGVÉNY ---
        private async Task<string> GetApiDataAsync(string endpoint)
        {
            string url = $"{BaseUrl}{endpoint}?key={ApiKey}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // VIP táblázat inicializálása (Tab 2)
        private void SetupVipGrid()
        {
            gridVip.Columns.Clear();
            gridVip.Columns.Add("Email", "Vásárló E-mail címe");
            gridVip.Columns.Add("OrderCount", "Rendelések száma");
            gridVip.Columns.Add("TotalSpent", "Összesen elköltve (Ft)");
            gridVip.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ==========================================
        // 💎 HIÁNYZÓ ESEMÉNYKEZELŐK ÉS VIP LOGIKA
        // ==========================================

        // Üres Form Load (a Designer keresi)
        private void Form1_Load(object sender, EventArgs e) { }

        // Üres gomb kattintás (valószínűleg dupla kattintással jött létre véletlenül)
        private void btnLoadColors_Click_1(object sender, EventArgs e) { }

        // A VIP Vásárlók betöltő gombja (Ezt véletlenül kivágtam az előbb!)
        private async void btnLoadVip_Click_1(object sender, EventArgs e)
        {
            try
            {
                btnLoadVip.Enabled = false;
                btnLoadVip.Text = "Kalkuláció folyamatban...";
                gridVip.Rows.Clear();

                string ordersJson = await GetApiDataAsync("orders");
                JObject ordersData = JObject.Parse(ordersJson);

                Dictionary<string, Tuple<int, decimal>> vasarloiStatisztika = new Dictionary<string, Tuple<int, decimal>>();

                foreach (var order in ordersData["Content"])
                {
                    string email = order["UserEmail"]?.ToString();
                    if (string.IsNullOrEmpty(email)) continue;

                    decimal orderTotal = 0;
                    if (order["TotalGrand"] != null)
                    {
                        decimal.TryParse(order["TotalGrand"].ToString(), out orderTotal);
                    }

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

                var topVasarlok = vasarloiStatisztika.OrderByDescending(x => x.Value.Item2).Take(10);

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
                btnLoadVip.Enabled = true;
            }
        }
    }
}