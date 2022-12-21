using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
