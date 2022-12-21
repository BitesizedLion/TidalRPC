using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

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
    }
}
