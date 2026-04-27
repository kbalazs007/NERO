using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

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
        private Chart chartWeeklyColors;

        // --- ADATSTRUKTÚRA A TÁBLÁZATHOZ ÉS ELEMZÉSHEZ ---
        public class ProductStat
        {
            public string ProductId { get; set; }
            public string Sku { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public int QuantitySold { get; set; }
            public decimal TotalRevenue { get; set; }
            public HashSet<string> OrderIdsContaining { get; set; } = new HashSet<string>();
        }

        // --- A VALÓDI SZÍNEK (RGB) SZÓTÁRA ---
        private readonly Dictionary<string, System.Drawing.Color> RealColors = new Dictionary<string, System.Drawing.Color>
        {
            { "Piros", System.Drawing.Color.FromArgb(220, 20, 40) },
            { "Bordó", System.Drawing.Color.FromArgb(120, 10, 30) },
            { "Rózsaszín", System.Drawing.Color.FromArgb(255, 150, 180) },
            { "Pink", System.Drawing.Color.FromArgb(230, 50, 130) },
            { "Nude", System.Drawing.Color.FromArgb(235, 205, 180) },
            { "Fehér", System.Drawing.Color.FromArgb(240, 240, 240) }, // Kicsit szürkés, hogy látszódjon a fehér háttéren
            { "Fekete", System.Drawing.Color.FromArgb(30, 30, 30) },
            { "Kék", System.Drawing.Color.FromArgb(50, 100, 200) },
            { "Lila", System.Drawing.Color.FromArgb(150, 80, 180) },
            { "Zöld", System.Drawing.Color.FromArgb(80, 180, 100) },
            { "Menta", System.Drawing.Color.FromArgb(160, 230, 190) },
            { "Sárga", System.Drawing.Color.FromArgb(250, 230, 80) },
            { "Narancs", System.Drawing.Color.FromArgb(240, 130, 40) },
            { "Barack", System.Drawing.Color.FromArgb(255, 180, 140) },
            { "Barna", System.Drawing.Color.FromArgb(140, 90, 60) },
            { "Szürke", System.Drawing.Color.FromArgb(150, 150, 150) },
            { "Arany", System.Drawing.Color.FromArgb(210, 180, 80) },
            { "Ezüst", System.Drawing.Color.FromArgb(190, 190, 190) },
            { "Neon", System.Drawing.Color.FromArgb(200, 255, 50) },
            { "Átlátszó", System.Drawing.Color.FromArgb(200, 220, 255) } // Halvány jégkék, hogy érzékeltesse
        };

        public Form1()
        {
            InitializeComponent();
            LoadSkuColors(); // CSV betöltése induláskor
            SetupVipGrid();
            SetupChart();

            SetupOverviewChart();

            btnLoadColors.Click += btnLoadColors_Click;
        }

        // --- CSV BETÖLTÉSE ---
        private void LoadSkuColors()
        {
            try
            {
                // CSV-hez
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sku_colors.csv");

                if (!File.Exists(path))
                {
                    MessageBox.Show("A 'sku_colors.csv' nem található a megadott útvonalon! A statisztika nem fog működni.", "Hiányzó adatfájl");
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

            // KIKAPCSOLJUK A DOCKOLÁST - Kézzel fogjuk elhelyezni!
            chartColors.Dock = DockStyle.None;

            // Beállítjuk a Horgonyokat, hogy kövesse az ablak méretezését
            chartColors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // PIXEL PONTOS POZÍCIONÁLÁS:
            chartColors.Top = 120; // Pontosan a felső panel alatt kezdődjön
            chartColors.Left = 0;
            chartColors.Width = tabPage1.Width;

            // A magassága = a fül teljes magassága MÍNUSZ a felső panel (120) MÍNUSZ az alsó panel (200)
            chartColors.Height = tabPage1.Height - 120 - 200;

            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.Area3DStyle.Enable3D = false;
            chartColors.ChartAreas.Add(chartArea);

            tabPage1.Controls.Add(chartColors);

            chartColors.Series.Clear();
            Series series = new Series("Színek");
            series.ChartType = SeriesChartType.Pie;

            series["PieLabelStyle"] = "Outside";

            chartColors.Series.Add(series);
            chartColors.Titles.Clear();
            chartColors.Titles.Add("Legnépszerűbb Színek Megoszlása (TOP 8)");

            // Táblázat oszlopainak beállítása
            gridTopProducts.Columns.Clear();
            gridTopProducts.Columns.Add("Name", "Termék neve");
            gridTopProducts.Columns.Add("Sku", "SKU");
            gridTopProducts.Columns.Add("Color", "Szín");
            gridTopProducts.Columns.Add("Qty", "Eladott db");
            gridTopProducts.Columns.Add("Revenue", "Bevétel ebből");

            gridTopProducts.Columns["Revenue"].DefaultCellStyle.Format = "N0";
            gridTopProducts.Columns["Revenue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridTopProducts.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupOverviewChart()
        {
            chartWeeklyColors = new Chart();

            // Kitölti az üres helyet
            chartWeeklyColors.Dock = DockStyle.Fill;

            ChartArea chartArea = new ChartArea("OverviewArea");
            chartArea.Area3DStyle.Enable3D = false;
            chartWeeklyColors.ChartAreas.Add(chartArea);

            // Hozzáadjuk a fülhöz (IDE ÍRD AZ ÚJ FÜLED NEVÉT!)
            tabPageOverview.Controls.Add(chartWeeklyColors);

            // Hátraküldjük, hogy ne takarja el a felső panelt a bevételekkel
            chartWeeklyColors.SendToBack();
        }

        // TabOverview

        private async Task LoadOverviewData()
        {
            try
            {
                // 1. Alapadatok és időablak beállítása (utolsó 7 nap)
                DateTime sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                decimal weeklyRevenue = 0;
                HashSet<string> weeklyUniqueCustomers = new HashSet<string>();
                Dictionary<string, int> weeklyColorStats = new Dictionary<string, int>();
                Dictionary<string, string> skuCache = new Dictionary<string, string>();

                string ordersJson = await GetApiDataAsync("orders");
                JObject ordersData = JObject.Parse(ordersJson);

                if (ordersData["Content"] is JArray ordersArray)
                {
                    foreach (JObject order in ordersArray.OfType<JObject>())
                    {
                        // Dátum kinyerése (a korábban javított, castolós módszerrel)
                        DateTime orderDate = DateTime.MinValue;
                        if (order["TimeOfOrderUtc"] != null)
                        {
                            orderDate = (DateTime)order["TimeOfOrderUtc"];
                        }

                        // CSAK AZ ELMÚLT 7 NAPOT NÉZZÜK
                        if (orderDate >= sevenDaysAgo)
                        {
                            // Bevétel és Vásárló számolás
                            decimal total = 0;
                            decimal.TryParse(order["TotalGrand"]?.ToString(), out total);
                            weeklyRevenue += total;

                            string email = order["UserEmail"]?.ToString();
                            if (!string.IsNullOrEmpty(email)) weeklyUniqueCustomers.Add(email);

                            // Szín statisztika a heti rendelések tételeiből
                            string orderBvin = order["bvin"]?.ToString() ?? order["Bvin"]?.ToString();
                            string detailJson = await GetApiDataAsync($"orders/{orderBvin}");
                            JObject fullOrder = (JObject)JObject.Parse(detailJson)["Content"];

                            JArray items = (fullOrder["Items"] as JArray) ?? (fullOrder["LineItems"] as JArray);
                            if (items != null)
                            {
                                foreach (JObject item in items.OfType<JObject>())
                                {
                                    string pid = item["ProductId"]?.ToString();
                                    int qty = 0;
                                    int.TryParse(item["Quantity"]?.ToString(), out qty);

                                    if (!string.IsNullOrEmpty(pid) && qty > 0)
                                    {
                                        string sku = "";
                                        if (skuCache.ContainsKey(pid)) sku = skuCache[pid];
                                        else
                                        {
                                            string pJson = await GetApiDataAsync($"products/{pid}");
                                            // VÉDŐHÁLÓ: Kényszerítjük, hogy objektumként kezelje. Ha sima szöveg/null jön, akkor a 'productContent' null lesz.
                                            JObject productContent = JObject.Parse(pJson)["Content"] as JObject;

                                            // A '?' operátor miatt ha a productContent null, akkor nem keres benne Sku-t, hanem békén hagyja és megy tovább.
                                            sku = productContent?["Sku"]?.ToString() ?? "";

                                            skuCache[pid] = sku;
                                        }

                                        if (SkuColorMap.ContainsKey(sku))
                                        {
                                            string color = SkuColorMap[sku];
                                            if (!weeklyColorStats.ContainsKey(color)) weeklyColorStats[color] = 0;
                                            weeklyColorStats[color] += qty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // 2. UI FRISSÍTÉS - KPI-ok
                lblWeeklyRevenue.Text = $"Heti forgalom: {weeklyRevenue:N0} Ft";
                lblWeeklyCustomers.Text = $"Heti vásárlók száma: {weeklyUniqueCustomers.Count} fő";

                // 3. UI FRISSÍTÉS - TOP 5 SZÍN DIAGRAM
                chartWeeklyColors.Series.Clear();
                chartWeeklyColors.Titles.Clear();
                chartWeeklyColors.Titles.Add("Heti legnépszerűbb 5 árnyalat");

                Series s = new Series("HetiSzinek") { ChartType = SeriesChartType.Column }; // Oszlopdiagram a változatosság kedvéért
                s.IsValueShownAsLabel = true;

                var top5 = weeklyColorStats.OrderByDescending(x => x.Value).Take(5);
                foreach (var stat in top5)
                {
                    int idx = s.Points.AddXY(stat.Key, stat.Value);
                    // Színezés a már meglévő RealColors szótárad alapján
                    if (RealColors.ContainsKey(stat.Key))
                    {
                        s.Points[idx].Color = RealColors[stat.Key];
                    }
                }
                chartWeeklyColors.Series.Add(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az Áttekintés betöltésekor: " + ex.Message);
            }
        }

        // TavOverView vége

        // --- SZÍNSTATISZTIKA GENERÁLÁSA ---
        private async void btnLoadColors_Click(object sender, EventArgs e)
        {
            try
            {
                btnLoadColors.Enabled = false;
                btnLoadColors.Text = "BI Elemzés futtatása...";
                chartColors.Series["Színek"].Points.Clear();
                gridTopProducts.Rows.Clear();

                if (SkuColorMap.Count == 0)
                {
                    MessageBox.Show("Nincsenek betöltött SKU adatok a CSV-ből!");
                    return;
                }

                string ordersSummaryJson = await GetApiDataAsync("orders");
                JObject ordersSummaryData = JObject.Parse(ordersSummaryJson);

                Dictionary<string, string> skuCache = new Dictionary<string, string>();
                Dictionary<string, ProductStat> productStats = new Dictionary<string, ProductStat>();
                Dictionary<string, HashSet<string>> colorOrdersMap = new Dictionary<string, HashSet<string>>();
                Dictionary<string, int> colorQuantityMap = new Dictionary<string, int>();

                // --- KPI Változók ---
                int totalValidOrders = 0;
                decimal totalRevenue = 0;
                int totalItemsSold = 0;

                // Trend változók (Elmúlt 30 nap vs Előző 30 nap)
                DateTime now = DateTime.UtcNow;
                int ordersLast30 = 0, ordersPrev30 = 0;

                if (ordersSummaryData["Content"] is JArray ordersArray)
                {
                    foreach (JObject orderSummary in ordersArray.OfType<JObject>())
                    {
                        decimal orderGrandTotal = 0;
                        if (orderSummary["TotalGrand"] != null) decimal.TryParse(orderSummary["TotalGrand"].ToString(), out orderGrandTotal);

                        if (orderGrandTotal > 0)
                        {
                            totalValidOrders++;
                            totalRevenue += orderGrandTotal;

                            string orderBvin = orderSummary["bvin"]?.ToString() ?? orderSummary["Bvin"]?.ToString();
                            DateTime orderDate = DateTime.MinValue;
                            if (orderSummary["TimeOfOrderUtc"] != null)
                            {
                                // Egyszerűen "kikényszerítjük" (castoljuk) a már konvertált dátumot!
                                orderDate = (DateTime)orderSummary["TimeOfOrderUtc"];
                            }

                            if (orderDate >= now.AddDays(-30)) ordersLast30++;
                            else if (orderDate >= now.AddDays(-60)) ordersPrev30++;

                            // Részletes kosár lekérése
                            string singleOrderJson = await GetApiDataAsync($"orders/{orderBvin}");
                            JObject fullOrder = (JObject)JObject.Parse(singleOrderJson)["Content"];

                            JArray items = (fullOrder["Items"] as JArray) ?? (fullOrder["LineItems"] as JArray);
                            if (items != null)
                            {
                                foreach (JObject item in items.OfType<JObject>())
                                {
                                    string productId = item["ProductId"]?.ToString();
                                    string productName = item["ProductName"]?.ToString();

                                    int qty = 0;
                                    if (item["Quantity"] != null) int.TryParse(item["Quantity"].ToString(), out qty);

                                    decimal lineTotal = 0;
                                    if (item["LineTotal"] != null) decimal.TryParse(item["LineTotal"].ToString(), out lineTotal);

                                    totalItemsSold += qty;

                                    if (!string.IsNullOrEmpty(productId) && qty > 0)
                                    {
                                        string valodiSku = "";
                                        if (skuCache.ContainsKey(productId))
                                        {
                                            valodiSku = skuCache[productId];
                                        }
                                        else
                                        {
                                            string singleProductJson = await GetApiDataAsync($"products/{productId}");
                                            JObject fullProduct = JObject.Parse(singleProductJson)["Content"] as JObject;
                                            valodiSku = fullProduct?["Sku"]?.ToString() ?? "";
                                            skuCache[productId] = valodiSku;
                                        }

                                        if (!string.IsNullOrEmpty(valodiSku) && SkuColorMap.ContainsKey(valodiSku))
                                        {
                                            string color = SkuColorMap[valodiSku];

                                            // Szín statisztika frissítése
                                            if (!colorQuantityMap.ContainsKey(color)) colorQuantityMap[color] = 0;
                                            colorQuantityMap[color] += qty;

                                            if (!colorOrdersMap.ContainsKey(color)) colorOrdersMap[color] = new HashSet<string>();
                                            colorOrdersMap[color].Add(orderBvin);

                                            // Termék statisztika frissítése a Táblázathoz
                                            if (!productStats.ContainsKey(productId))
                                            {
                                                productStats[productId] = new ProductStat { ProductId = productId, Name = productName, Sku = valodiSku, Color = color };
                                            }
                                            productStats[productId].QuantitySold += qty;
                                            productStats[productId].TotalRevenue += lineTotal;
                                            productStats[productId].OrderIdsContaining.Add(orderBvin);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // --- 1. KPI-ok KIÍRÁSA A CÍMKÉKRE ---
                decimal avgOrderValue = totalValidOrders > 0 ? (totalRevenue / totalValidOrders) : 0;
                string trendText = "";
                if (ordersPrev30 > 0)
                {
                    double diff = ((double)(ordersLast30 - ordersPrev30) / ordersPrev30) * 100;
                    trendText = $" (Trend: {diff:+0;-0}% vs előző hónap)";
                }

                lblTotalSold.Text = $"Eladott darab (összes): {totalItemsSold} db";
                lblTotalRevenue.Text = $"Bevétel: {totalRevenue:N0} Ft";
                lblAvgOrder.Text = $"Átlagos kosárérték: {avgOrderValue:N0} Ft\nRendelések: {ordersLast30} db elmúlt 30 napban{trendText}";

                // --- 2. KÖRDIAGRAM FRISSÍTÉSE ÉS SZÍNEZÉSE (TOP 8 Szín) ---
                var topColors = colorQuantityMap.OrderByDescending(x => x.Value).Take(8).ToList();

                // !!! JAVÍTÁS INDUL !!!
                // 1. Kiszámoljuk a CSAK a TOP 8 elemhez tartozó darabszámok összegét.
                // Ezzel fogunk osztani, hogy a torta 100%-ot adjon ki.
                int top8OsszesDarab = topColors.Sum(x => x.Value);

                // Biztonsági ellenőrzés (bár elvileg nem lehet 0, de jobb félni)
                if (top8OsszesDarab > 0)
                {
                    for (int i = 0; i < topColors.Count; i++)
                    {
                        string szinNeve = topColors[i].Key;
                        int darabszam = topColors[i].Value;

                        // Hozzáadjuk a pontot. Az X-et 0-nak hagyjuk, mert manuálisan feliratozzuk!
                        int ptIndex = chartColors.Series["Színek"].Points.AddXY(0, darabszam);
                        DataPoint pt = chartColors.Series["Színek"].Points[ptIndex];

                        // SZÍNEZÉS: Rárakjuk a tortaszeletre a valódi RGB színt!
                        if (RealColors.ContainsKey(szinNeve))
                        {
                            pt.Color = RealColors[szinNeve];
                        }

                        // JAVÍTOTT FELIRATOZÁS
                        // A 'totalItemsSold' helyett a 'top8OsszesDarab' -al osztunk!
                        double szazalekA_Tortaban = ((double)darabszam / top8OsszesDarab) * 100;
                        pt.Label = $"{szinNeve}\n{szazalekA_Tortaban:0.0}% ({darabszam} db)";

                        // Jelmagyarázat (Legend) - Ezt változatlanul hagyjuk, mert hasznos adat
                        int rendelesekAmibenBenneVolt = colorOrdersMap[szinNeve].Count;
                        double reszesedesARendelesekbol = ((double)rendelesekAmibenBenneVolt / totalValidOrders) * 100;
                        pt.LegendText = $"{szinNeve} - Kosarak {reszesedesARendelesekbol:0.0}%-ában";
                    }
                }
                else
                {
                    MessageBox.Show("Nincs megjeleníthető adat a diagramhoz.", "Nincs adat");
                }
                // !!! JAVÍTÁS VÉGE !!!

                // Csel: A Legend-be (jelmagyarázatba) beleírjuk, hogy a rendelések hány %-ában volt benne!
                for (int i = 0; i < topColors.Count; i++)
                {
                    string szinNeve = topColors[i].Key;
                    int darabszam = topColors[i].Value;
                    int rendelesekAmibenBenneVolt = colorOrdersMap[szinNeve].Count;
                    double reszesedesARendelesekbol = ((double)rendelesekAmibenBenneVolt / totalValidOrders) * 100;

                    chartColors.Series["Színek"].Points[i].LegendText = $"{szinNeve} ({darabszam} db) - Kosarak {reszesedesARendelesekbol:0.0}%-ában";
                }

                // --- 3. TÁBLÁZAT FELTÖLTÉSE (TOP Termékek) ---
                var topProducts = productStats.Values.OrderByDescending(x => x.TotalRevenue).Take(15);
                foreach (var prod in topProducts)
                {
                    gridTopProducts.Rows.Add(prod.Name, prod.Sku, prod.Color, prod.QuantitySold, prod.TotalRevenue);
                }

                btnLoadColors.Text = "Sikeres Elemzés!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a BI elemzés közben:\n{ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Opcionális: Ha van más betöltendő dolog, az is jöhet ide
            await LoadOverviewData();
        }

        // Üres gomb kattintás (valószínűleg dupla kattintással jött létre véletlenül)
        private void btnLoadColors_Click_1(object sender, EventArgs e) { }

        // A VIP Vásárlók betöltő gombja
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