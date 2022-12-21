using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace TidalRPC
{
    public static class Utils
    {
        // Method for checking if the coreProps.json file exists, if it doesn't, creates it.
        public static void CheckCoreProps()
        {
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string filePath = Path.Combine(programDataPath, "SteelSeries/SteelSeries Engine 3/coreProps.json");
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("CheckCoreProps: directory does not exist, creating it");
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("CheckCoreProps: coreProps does not exist, creating it");
                string contents = "{\"address\":\"127.0.0.1:3650\"}";
                File.WriteAllText(filePath, contents);
            } else { Console.WriteLine("CheckCoreProps: coreProps was found"); }
        }

        // Method for getting the address
        public static string GetAddress()
        {
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string filePath = Path.Combine(programDataPath, "SteelSeries/SteelSeries Engine 3/coreProps.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("GetAddress: coreProps does not exist, returning default address");
                return "http://127.0.0.1:3650/";
            }

            string jsonString = File.ReadAllText(filePath);
            dynamic json = JsonConvert.DeserializeObject(jsonString);
            string address = $"http://{json["address"]}/";

            return address;
        }

        // Prefixes 
        public class PrefixedWriter : TextWriter
        {
            private TextWriter originalOut;

            public PrefixedWriter()
            {
                originalOut = Console.Out;
            }

            public override Encoding Encoding
            {
                get { return new ASCIIEncoding(); }
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

        public static class RegistryManager // i know technically they arent keys
        {
            //public static object GetKeyValue(string valueName)
            //{
            //    string keyName = "SOFTWARE\\TidalRPC\\Settings";

            //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            //    {
            //        if (key == null) return null;

            //        return key.GetValue(valueName, 0);
            //    }
            //}

            //public static void SetKeyValue(string valueName, object value, RegistryValueKind type)
            //{
            //    string keyName = "SOFTWARE\\TidalRPC\\Settings";

            //    RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
            //    if (key == null)
            //    {
            //        // Create the registry key if it does not exist
            //        key = Registry.CurrentUser.CreateSubKey(keyName);
            //    }

            //    key.SetValue(valueName, value, type);
            //}

            //// This method fetches the value of a registry key and returns true if the value is 1, and false if the value is 0
            //public static bool IsRegistryKeyEnabled(string valueName, bool fallback)
            //{
            //    string keyName = "SOFTWARE\\TidalRPC\\Settings";

            //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            //    {
            //        if (key == null)
            //        {
            //            ToggleRegistryKey(fallback);
            //            return fallback;
            //        }

            //        return (int)key.GetValue(valueName, 0) != 0;
            //    }
            //}
            //public static bool IsRegistryKeyEnabled(string valueName)
            //{
            //    string keyName = "SOFTWARE\\TidalRPC\\Settings";

            //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
            //    {
            //        if (key != null) return true;

            //        return (int)key.GetValue(valueName, 0) != 0;
            //    }
            //}

            //// This method sets a specified registry key to the provided boolean value (but as 0 or 1)
            //public static void ToggleRegistryKey(string valueName)
            //{
            //    string keyName = "SOFTWARE\\TidalRPC\\Settings";

            //    RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
            //    if (key == null)
            //    {
            //        // Create the registry key if it does not exist
            //        key = Registry.CurrentUser.CreateSubKey(keyName);
            //    }

            //    key.SetValue(valueName, Convert.ToInt32(!IsRegistryKeyEnabled(valueName)), RegistryValueKind.DWord);
            //}
        }
    }
}
