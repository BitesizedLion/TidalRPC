using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TidalRPC
{
    public class Utils
    {
        private static readonly string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private static readonly string corePropsPath = Path.Combine(programDataPath, "SteelSeries/SteelSeries Engine 3/coreProps.json");

        // Method for checking if the coreProps.json file exists, if it doesn't, creates it.
        public static void CheckCoreProps()
        {
            string directoryPath = Path.GetDirectoryName(corePropsPath);

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("CheckCoreProps: directory does not exist, creating it");
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(corePropsPath))
            {
                Console.WriteLine("CheckCoreProps: coreProps does not exist, creating it");
                
                string contents = "{\"address\":\"127.0.0.1:3650\"}";

                File.WriteAllText(corePropsPath, contents);
            } else { Console.WriteLine("CheckCoreProps: coreProps was found"); }
        }

        // Method for getting the address
        public static string GetAddress()
        {
            if (!File.Exists(corePropsPath))
            {
                Console.WriteLine("GetAddress: coreProps does not exist, returning default address");
                return "http://127.0.0.1:3650/";
            }

            string jsonString = File.ReadAllText(corePropsPath);
            CoreProps json = JsonConvert.DeserializeObject<CoreProps>(jsonString);
            string address = $"http://{json.Address}/";

            return address;
        }

        // Prefixes the console logs as `[date] message`
        public class TimestampedWriter : TextWriter
        {
            private readonly TextWriter originalOut;

            public TimestampedWriter()
            {
                originalOut = Console.Out;
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
            public override void WriteLine(string message)
            {
                originalOut.WriteLine(string.Format("[{0}] {1}", DateTime.Now, message));
            }
            public override void Write(string message)
            {
                originalOut.Write(string.Format("[{0}] {1}", DateTime.Now, message));
            }
        }

        // Class for coreProps.json
        public class CoreProps
        {
            public string Address { get; set; }
        }

        public static class CountryCodeUtility
        {
            public static bool IsCountryCodeValid(string countryCode)
            {
                return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                  .Select(culture => new RegionInfo(culture.LCID))
                                  .Any(region => region.TwoLetterISORegionName.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
            }

            public static string PromptForValidCountryCode()
            {
                while (true)
                {
                    string userInput = PromptInputBox("Please enter a two-character ISO country code:", "Enter Country Code", "");

                    if (IsCountryCodeValid(userInput))
                    {
                        return SetCountryCode(userInput);
                    }
                    else
                    {
                        MessageBox.Show("Invalid country code. Please enter a valid two-character ISO country code.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            // Yes, it's awful but I'm not fixing it. "If it works, don't touch it"
            private static string PromptInputBox(string prompt, string title, string defaultValue)
            {
                Form promptForm = new Form()
                {
                    Width = 300,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = title,
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label label = new Label() { Left = 20, Top = 20, Width = 260, Text = prompt };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 260, Text = defaultValue };
                Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 60, Top = 80, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancel", Left = 220, Width = 60, Top = 80, DialogResult = DialogResult.Cancel };

                confirmation.Click += (sender, e) => promptForm.Close();
                cancel.Click += (sender, e) => promptForm.Close();

                promptForm.Controls.Add(textBox);
                promptForm.Controls.Add(confirmation);
                promptForm.Controls.Add(cancel);
                promptForm.Controls.Add(label);

                promptForm.AcceptButton = confirmation;
                promptForm.CancelButton = cancel;

                return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "US";
            }

            // Method for getting the country code
            public static string GetCountryCode()
            {
                string keyName = "SOFTWARE\\TidalRPC\\Settings";
                string valueName = "CountryCode";

                using RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName);

                if (key == null)
                {
                    SetCountryCode("US"); // default to US
                    return "US";
                }

                return (string)key.GetValue(valueName, "US");
            }

            // Method for setting the country code
            private static string SetCountryCode(string countryCode)
            {
                string keyName = "SOFTWARE\\TidalRPC\\Settings";
                string valueName = "CountryCode";

                RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);

                if (key == null) key = Registry.CurrentUser.CreateSubKey(keyName); // Create the registry key if it does not exist

                key.SetValue(valueName, countryCode, RegistryValueKind.String);

                return countryCode; // Return the countryCode because testing, TODO: REMOVE
            }
        }
    }
}
