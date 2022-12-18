using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TidalRPC
{
    public static class UpdateManager
    {
        static bool updateDenied = false;

        // Method for checking if update checking is enabled
        public static bool IsUpdateCheckEnabled()
        {
            string keyName = "SOFTWARE\\TidalRPC\\Settings";
            string valueName = "CheckForUpdates";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key == null)
                {
                    ToggleUpdateCheck(true);
                    return true;
                }

                return (int)key.GetValue(valueName, 0) != 0;
            }
        }

        // Method for toggling update checking
        public static void ToggleUpdateCheck(bool checkForUpdates)
        {
            string keyName = "SOFTWARE\\TidalRPC\\Settings";
            string valueName = "CheckForUpdates";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
            if (key == null)
            {
                // Create the registry key if it does not exist
                key = Registry.CurrentUser.CreateSubKey(keyName);
            }

            key.SetValue(valueName, Convert.ToInt32(checkForUpdates), RegistryValueKind.DWord);
        }

        // Method for checking for new updates
        public static async Task CheckForUpdates()
        {
            while (!updateDenied)
            {
                if (!IsUpdateCheckEnabled()) return;

                // Get the current application version
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                // Fetch the latest release information from the GitHub API
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("tidalrpc/" + currentVersion.ToString()); // GH api needs custom user-agent
                HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/BitesizedLion/TidalRPC/releases/latest");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic release = JsonConvert.DeserializeObject(json);
                    Version latestVersion = new Version(((string)release.tag_name).Replace("v", ""));

                    // Compare the version numbers
                    if (latestVersion > currentVersion)
                    {
                        // Prompt the user to update
                        DialogResult result = MessageBox.Show("A new version of the software is available. Do you want to download it?", "Update Available", MessageBoxButtons.YesNo);
                        
                        switch (result)
                        {
                            case DialogResult.Yes:
                                Process.Start((string)release.html_url); // Open the download URL in the default web browser
                                break;

                            case DialogResult.No:
                                updateDenied = true;
                                break;

                        }
                    }
                }
                else { Console.WriteLine("releases req failed: " + response.StatusCode); }

                // Wait for an hour before checking for updates again
                await Task.Delay(TimeSpan.FromHours(1));
            }
        }
    }
}
