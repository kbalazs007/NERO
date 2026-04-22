using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace HotcakesAdminPro // Ha te máshogy nevezted el a projektet, ezt a sort írd át arra!
{
    public partial class Form1 : Form
    {
        // --- A TE SAJÁT SZERVERED ÉS KULCSOD ---
        private const string BaseUrl = "http://nerokozmetika.hu/DesktopModules/Hotcakes/API/rest/v1/";
        private const string ApiKey = "1-3518c8a8-8059-47bf-9918-5dc030e1ad34";

        // A HttpClient intézi a hálózati kommunikációt
        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        // --- A GOMB KATTINTÁS ESEMÉNYE ---
        private async void btnTestApi_Click(object sender, EventArgs e)
        {
            try
            {
                // Gomb letiltása, hogy ne lehessen 100x rákattintani amíg tölt
                btnTestApi.Enabled = false;
                txtConsole.Text = "Kapcsolódás a Hotcakes VM-hez...\r\n";

                // 1. Összerakjuk a lekérdezést (Kategóriák listázása)
                string endpoint = "categories";
                string fullUrl = $"{BaseUrl}{endpoint}?key={ApiKey}";

                txtConsole.Text += $"Cél URL: {fullUrl}\r\nFolyamatban...\r\n";

                // 2. Elküldjük a GET kérést a szervernek
                HttpResponseMessage response = await client.GetAsync(fullUrl);

                // 3. Ellenőrizzük, hogy nincs-e hiba (pl. rossz kulcs = 401 Unauthorized)
                response.EnsureSuccessStatusCode();

                // 4. Beolvassuk a szerver válaszát szövegként
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // 5. A Newtonsoft.Json segítségével "megszépítjük", hogy olvasható legyen
                JObject parsedJson = JObject.Parse(jsonResponse);

                txtConsole.Text += "\r\n✅ SIKERES KAPCSOLAT! Az API él és válaszol.\r\n";
                txtConsole.Text += "--------------------------------------------------\r\n";
                txtConsole.Text += parsedJson.ToString(); // Kiírjuk a formázott JSON-t
            }
            catch (Exception ex)
            {
                // Ha bármi beszakad (nincs net, rossz az URL, leállt a VM), itt írja ki a hibát
                txtConsole.Text += $"\r\n❌ HIBA TÖRTÉNT A KAPCSOLÓDÁS SORÁN:\r\n{ex.Message}";
            }
            finally
            {
                // Visszakapcsoljuk a gombot
                btnTestApi.Enabled = true;
            }
        }
    }
}