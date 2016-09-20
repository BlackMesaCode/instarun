using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InstaRun
{
    public class ConfigManager
    {
        public static readonly string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExeDir = Path.GetDirectoryName(ExePath);
        public static readonly string ConfigFileName = "Config.xml";
        public static readonly string SampleConfigFileName = "Config.Sample.xml";

        public Config CreateSampleConfig()
        {
            var settings = new Settings();
            var items = new List<Item>()
            {
                new Executable("VSCode", "code ."),
                new Executable("Explorer", "explorer.exe"),
                new Seperator(),
                new Container("Browsers", new List<Item>() {
                    new Executable("Internet Explorer", "iexplore"),
                    new Executable("Chrome", "chrome"),
                    new Container("Test Browsers", new List<Item>() {
                        new Executable("Firefox", "firefox"),
                    }),
                }),
            };
            var config = new Config(settings, items);
            return config;
        }

        public Config GetConfig()
        {
            var path = Path.Combine(ExeDir, ConfigFileName);
            return Deserialize<Config>(path);
        }

        public void CreateSampleConfigXml()
        {
            var path = Path.Combine(ExeDir, SampleConfigFileName);
            var objectToSerialize = CreateSampleConfig();

            Serialize<Config>(objectToSerialize, path);
        }


        public static T Deserialize<T>(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T result = default(T);
                using (var reader = new StreamReader(path))
                {
                    result = (T)xs.Deserialize(reader);
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static void Serialize<T>(T objectToSerialize, string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            var writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
            };

            using (var writer = XmlWriter.Create(path, writerSettings))
            {
                xs.Serialize(writer, objectToSerialize);
            }
        }

    }
}
